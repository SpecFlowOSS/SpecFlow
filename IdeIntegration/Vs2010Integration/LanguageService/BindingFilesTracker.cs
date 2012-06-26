using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class BindingAssemblyInfo
    {
        public string AssemblyName { get; private set; }
        public Project Project { get; private set; }

        public bool IsProject
        {
            get { return Project != null; }
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

        public BindingAssemblyInfo(string assemblyName, DTE dte)
        {
            AssemblyName = assemblyName;
            Project = VsxHelper.FindProjectByAssemblyName(dte, AssemblyShortName);
        }
    }

    public class BindingFileInfo : FileInfo
    {
        public IEnumerable<StepDefinitionBinding> StepBindings { get; set; }

        public BindingFileInfo(ProjectItem projectItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            LastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
        }
    }

    internal class BindingFilesTracker : ProjectFilesTracker<BindingFileInfo>, IDisposable
    {
        private static readonly string[] bindingFileExtensions = new[] {"cs", "vb" };

        private Dictionary<BindingAssemblyInfo, VsProjectFilesTracker> filesTracker;
        private readonly VsStepSuggestionBindingCollector stepSuggestionBindingCollector;
        private VsProjectReferencesTracker projectReferencesTracker;

        public BindingFilesTracker(VsProjectScope vsProjectScope) : base(vsProjectScope)
        {
            stepSuggestionBindingCollector = new VsStepSuggestionBindingCollector();
        }

        public override void Initialize()
        {
            var mainProject = vsProjectScope.Project;
            var bindingAssemblies = Enumerable.Empty<BindingAssemblyInfo>()
                .Append(new BindingAssemblyInfo(mainProject))
                .Concat(vsProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.AdditionalStepAssemblies.Select(
                        assembly => new BindingAssemblyInfo(assembly, mainProject.DTE)).Where(ai => ai.Project != mainProject));

            filesTracker = bindingAssemblies.ToDictionary(
                ai => ai,
                ai => ai.IsProject ? CreateFilesTracker(ai) : null);

            projectReferencesTracker = new VsProjectReferencesTracker(mainProject, vsProjectScope.DteWithEvents, vsProjectScope.Tracer);
            projectReferencesTracker.StartTracking();

            base.Initialize();
        }

        private VsProjectFilesTracker CreateFilesTracker(BindingAssemblyInfo ai)
        {
            return CreateFilesTracker(ai.Project, @"\.(" + string.Join("|", bindingFileExtensions) + @")$");
        }

        protected override void Analyze(BindingFileInfo fileInfo, ProjectItem projectItem)
        {
            vsProjectScope.Tracer.Trace("Analyzing binding file: " + fileInfo.ProjectRelativePath, "BindingFilesTracker");
            fileInfo.StepBindings = stepSuggestionBindingCollector.GetBindingsFromProjectItem(projectItem).ToArray();
            fileInfo.LastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
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
                var extension = Path.GetExtension(projectItem.Name);
                if (!bindingFileExtensions.Any(bindingExt => ("." + bindingExt).Equals(extension, StringComparison.InvariantCultureIgnoreCase)))
                    return false;

                return VsxHelper.GetClasses(projectItem).Any(VsStepSuggestionBindingCollector.IsBindingClass);
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override void SaveToStepMapInternal(StepMap stepMap)
        {
            stepMap.ProjectStepDefinitions = new List<ProjectStepDefinitions>();
            var projectStepDefinitions = new ProjectStepDefinitions();
            projectStepDefinitions.FileStepDefinitions = new List<FileStepDefinitions>();
            stepMap.ProjectStepDefinitions.Add(projectStepDefinitions);
            foreach (var bindingFileInfo in Files.Where(f => f.IsAnalyzed))
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
