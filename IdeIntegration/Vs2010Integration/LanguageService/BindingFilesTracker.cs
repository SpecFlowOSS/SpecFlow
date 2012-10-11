using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.IdeIntegration.Bindings;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Bindings.Discovery;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using VSLangProj;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class BindingAssemblyInfo
    {
        public string AssemblyName { get; private set; }
        public Project Project { get; private set; }
        public Reference Reference { get; private set; }

        public bool IsProject
        {
            get { return Project != null; }
        }

        public bool IsAssemblyReference
        {
            get { return Reference != null; }
        }

        public string AssemblyShortName
        {
            get { return AssemblyName.Split(new[] { ',' }, 2)[0].Trim(); }
        }

        public BindingAssemblyInfo(Project project)
        {
            AssemblyName = project.GetAssemblyName();
            Project = project;
        }

        public BindingAssemblyInfo(string assemblyName, Project mainProject)
        {
            AssemblyName = assemblyName;
            Project = VsxHelper.FindProjectByAssemblyName(mainProject.DTE, AssemblyShortName);

            if (Project != null && VsProjectScope.GetTargetLanguage(Project) == ProgrammingLanguage.FSharp) //HACK: we force the f# libs to be used as assembly reference
                Project = null;

            if (Project == null)  
                Reference = VsxHelper.GetReference(mainProject, assemblyName);
        }
    }

    public class BindingFileInfo : FileInfo
    {
        public IEnumerable<IStepDefinitionBinding> StepBindings { get; set; }

        public bool IsAssembly
        {
            get
            {
                var extension = Path.GetExtension(ProjectRelativePath);
                return (".dll".Equals(extension, StringComparison.InvariantCultureIgnoreCase) ||
                        ".exe".Equals(extension, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public BindingFileInfo(ProjectItem projectItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            LastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
        }

        public BindingFileInfo(Reference reference)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(reference);
            LastChangeDate = VsxHelper.GetLastChangeDate(reference) ?? DateTime.MinValue;
        }
    }

    internal class BindingFilesTracker : ProjectFilesTracker<BindingFileInfo>, IDisposable
    {
        private static readonly string[] bindingFileExtensions = new[] {"cs", "vb" };

        private Dictionary<BindingAssemblyInfo, VsProjectFilesTracker> filesTracker;
        private readonly VsBindingRegistryBuilder stepSuggestionBindingCollector;
        private VsProjectReferencesTracker projectReferencesTracker;

        public BindingFilesTracker(VsProjectScope vsProjectScope) : base(vsProjectScope)
        {
            stepSuggestionBindingCollector = new VsBindingRegistryBuilder(vsProjectScope.Tracer);
        }

        public IEnumerable<BindingAssemblyInfo> BindingAssemblies
        {
            get { return filesTracker.Keys; }
        }

        public override void Initialize()
        {
            var mainProject = vsProjectScope.Project;

            var bindingAssemblies = Enumerable.Empty<BindingAssemblyInfo>()
                .Append(new BindingAssemblyInfo(mainProject))
                .Concat(vsProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.AdditionalStepAssemblies
                            .Select(assembly => new BindingAssemblyInfo(assembly, mainProject)).ToArray().Where(ai => ai.Project != mainProject)).ToList();

            filesTracker = bindingAssemblies.ToDictionary(
                ai => ai,
                ai => ai.IsProject ? CreateFilesTracker(ai) : null);
            
            projectReferencesTracker = new VsProjectReferencesTracker(mainProject, vsProjectScope.DteWithEvents, vsProjectScope.Tracer);
            projectReferencesTracker.StartTracking();
            projectReferencesTracker.ReferenceChanged += ProjectReferencesTrackerOnReferenceChanged;
            projectReferencesTracker.ReferenceAdded += ProjectReferencesTrackerOnReferenceChanged;
            projectReferencesTracker.ReferenceRemoved += ProjectReferencesTrackerOnReferenceRemoved;

            base.Initialize();
        }

        private void ProjectReferencesTrackerOnReferenceRemoved(Reference reference)
        {
            if (!IsInitialized)
                return;

            var fileInfo = FindFileInfo(VsxHelper.GetProjectRelativePath(reference));
            if (fileInfo != null)
            {
                RemoveFileInfo(fileInfo);
            }
        }

        private void ProjectReferencesTrackerOnReferenceChanged(Reference reference)
        {
            if (!IsInitialized)
                return;

            var bindingAsssembly = BindingAssemblies.FirstOrDefault(ba => ba.AssemblyName.Equals(reference.Name, StringComparison.InvariantCultureIgnoreCase));
            bool isInScope = bindingAsssembly != null && !bindingAsssembly.IsProject;
            var fileInfo = FindFileInfo(VsxHelper.GetProjectRelativePath(reference));
            if (fileInfo != null)
            {
                if (isInScope)
                    ChangeFile(fileInfo);
                else
                    RemoveFileInfo(fileInfo);
            }
            else if (isInScope)
            {
                fileInfo = new BindingFileInfo(reference);
                AddFileInfo(fileInfo);
            }
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();

            files.AddRange(BindingAssemblies.Where(ba => ba.IsAssemblyReference).Select(ba => new BindingFileInfo(ba.Reference)));
        }

        private VsProjectFilesTracker CreateFilesTracker(BindingAssemblyInfo ai)
        {
            return CreateFilesTracker(ai.Project, @"\.(" + string.Join("|", bindingFileExtensions) + @")$");
        }

        protected override bool HandleMissingProjectItem(BindingFileInfo fileInfo)
        {
            if (fileInfo.IsAssembly)
                return false; // this is a hack, the assembly references do not have a project item

            return base.HandleMissingProjectItem(fileInfo);
        }

        protected override void Analyze(BindingFileInfo fileInfo, ProjectItem projectItem)
        {
            vsProjectScope.Tracer.Trace("Analyzing binding file: {0}", this, fileInfo.ProjectRelativePath);

            if (fileInfo.IsAssembly)
            {
                AnalyzeAssembly(fileInfo);
            }
            else
            {
                fileInfo.StepBindings = stepSuggestionBindingCollector.GetBindingsFromProjectItem(projectItem).ToArray();
                fileInfo.LastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
            }
        }

        private void AnalyzeAssembly(BindingFileInfo fileInfo)
        {
            var reference = VsxHelper.GetReferenceByProjectRelativePath(vsProjectScope.Project, fileInfo.ProjectRelativePath);
            if (reference == null)
                throw new InvalidOperationException("Could not find project reference for path: " + fileInfo.ProjectRelativePath);

            vsProjectScope.Tracer.Trace("Calculate step definitions from assembly: {0}", this, reference.Name);

            ILBindingRegistryBuilder builder = new ILBindingRegistryBuilder();
            fileInfo.StepBindings = builder.GetStepDefinitionsFromAssembly(reference.Path).ToArray();

            vsProjectScope.Tracer.Trace("Detected {0} step definitions from reference {1}", this, fileInfo.StepBindings.Count(), reference.Name);

            fileInfo.LastChangeDate = VsxHelper.GetLastChangeDate(reference) ?? DateTime.MinValue;
        }

        protected override BindingFileInfo CreateFileInfo(ProjectItem projectItem)
        {
            return new BindingFileInfo(projectItem);
        }

        protected override IEnumerable<Project> GetProjects()
        {
            return filesTracker.Keys.Where(ai => ai.IsProject).Select(ai => ai.Project);
        }

        protected override bool IsMatchingProjectItem(ProjectItem projectItem)
        {
            try
            {
                return IsMatchingExtension(projectItem);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsMatchingExtension(ProjectItem projectItem)
        {
            var extension = Path.GetExtension(projectItem.Name);
            return bindingFileExtensions.Any(bindingExt => ("." + bindingExt).Equals(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override void SaveToStepMapInternal(StepMap stepMap)
        {
            stepMap.ProjectStepDefinitions = new List<ProjectStepDefinitions>();
            var projectStepDefinitions = new ProjectStepDefinitions();
            projectStepDefinitions.FileStepDefinitions = new List<FileStepDefinitions>();
            stepMap.ProjectStepDefinitions.Add(projectStepDefinitions);
            foreach (var bindingFileInfo in Files.Where(f => f.IsAnalyzed && !f.IsError))
            {
                var fileStepDefinitions = new FileStepDefinitions
                                              {
                                                  FileName = bindingFileInfo.ProjectRelativePath, 
                                                  TimeStamp = bindingFileInfo.LastChangeDate
                                              };

                fileStepDefinitions.StepDefinitions = new List<StepDefinitionBindingItem>();
                fileStepDefinitions.StepDefinitions.AddRange(bindingFileInfo.StepBindings.Select(StepDefinitionBindingItem.FromStepDefinitionBinding));
                projectStepDefinitions.FileStepDefinitions.Add(fileStepDefinitions);
            }
        }

        public void Dispose()
        {
            if (filesTracker != null)
            {
                foreach (var vsProjectFilesTracker in filesTracker.Values.Where(t => t != null))
                    DisposeFilesTracker(vsProjectFilesTracker);
                filesTracker.Clear();
            }
            if (projectReferencesTracker != null)
            {
                projectReferencesTracker.Dispose();
                projectReferencesTracker = null;
            }
        }

        protected override void LoadFromStepMapInternal(StepMap stepMap)
        {
            if (stepMap.ProjectStepDefinitions == null || stepMap.ProjectStepDefinitions.Count == 0)
                return;

            var projectStepDefitions = stepMap.ProjectStepDefinitions[0];

            foreach (var fileStepDefinitions in projectStepDefitions.FileStepDefinitions)
            {
                try
                {
                    var fileInfo = FindFileInfo(fileStepDefinitions.FileName);
                    if (fileInfo == null)
                        continue;

                    if (fileInfo.IsDirty(fileStepDefinitions.TimeStamp))
                        continue;

                    fileInfo.StepBindings = fileStepDefinitions.StepDefinitions.Select(bi => bi.ToStepDefinitionBinding()).ToList();

                    FireFileUpdated(fileInfo);
                    fileInfo.IsError = false;
                    fileInfo.IsAnalyzed = true;
                }
                catch(Exception ex)
                {
                    vsProjectScope.Tracer.Trace(string.Format("Binding load error for {0}: {1}", fileStepDefinitions.FileName, ex), GetType().Name);
                }
            }

            vsProjectScope.Tracer.Trace("Applied loaded bindings", GetType().Name);
        }
    }
}
