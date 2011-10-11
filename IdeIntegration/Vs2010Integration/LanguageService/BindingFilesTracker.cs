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

    public class BindingFileInfo : IFileInfo
    {
        public string ProjectRelativePath { get; private set; }
        public bool IsAnalyzed { get; set; }
        public IEnumerable<StepBindingNew> StepBindings { get; set; }

        public BindingFileInfo(ProjectItem projectItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
        }

        public void Rename(string newProjectRelativePath)
        {
            ProjectRelativePath = newProjectRelativePath;
        }
    }

    internal class BindingFilesTracker : ProjectFilesTracker<BindingFileInfo>, IDisposable
    {
        private Dictionary<BindingAssemblyInfo, VsProjectFilesTracker> filesTracker;
        private readonly VsStepSuggestionBindingCollector stepSuggestionBindingCollector;

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

            base.Initialize();
        }

        private VsProjectFilesTracker CreateFilesTracker(BindingAssemblyInfo ai)
        {
            return CreateFilesTracker(ai.Project, @"\.(cs|vb)$");
        }

        protected override void Analyze(BindingFileInfo fileInfo, ProjectItem projectItem)
        {
            vsProjectScope.VisualStudioTracer.Trace("Analyzing binding file: " + fileInfo.ProjectRelativePath, "BindingFilesTracker");
            fileInfo.StepBindings = stepSuggestionBindingCollector.GetBindingsFromProjectItem(projectItem).ToArray();
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
                if (!".cs".Equals(extension, StringComparison.InvariantCultureIgnoreCase) &&
                    !".vb".Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                return VsxHelper.GetClasses(projectItem).Any(VsStepSuggestionBindingCollector.IsBindingClass);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveToStepMap(StepMap stepMap)
        {
            stepMap.ProjectStepDefinitions = new List<ProjectStepDefinitions>();
            var projectStepDefinitions = new ProjectStepDefinitions();
            projectStepDefinitions.FileStepDefinitions = new List<FileStepDefinitions>();
            stepMap.ProjectStepDefinitions.Add(projectStepDefinitions);
            foreach (var bindingFileInfo in Files.Where(f => f.IsAnalyzed))
            {
                var fileStepDefinitions = new FileStepDefinitions {FileName = bindingFileInfo.ProjectRelativePath};

                fileStepDefinitions.StepDefinitions = new List<StepBindingNew>();
                fileStepDefinitions.StepDefinitions.AddRange(bindingFileInfo.StepBindings);
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
        }

        public void LoadFromStepMap(StepMap stepMap)
        {
            DoTaskAsynch(() => LoadFromStepMapInternal(stepMap));
        }

        private void LoadFromStepMapInternal(StepMap stepMap)
        {
            if (stepMap.ProjectStepDefinitions == null || stepMap.ProjectStepDefinitions.Count == 0)
                return;

            var projectStepDefitions = stepMap.ProjectStepDefinitions[0];

            foreach (var fileStepDefinitions in projectStepDefitions.FileStepDefinitions)
            {
                var fileInfo = FindFileInfo(fileStepDefinitions.FileName);
                if (fileInfo == null)
                    continue;

                fileInfo.StepBindings = fileStepDefinitions.StepDefinitions;
                fileInfo.IsAnalyzed = true;
            }
        }
    }
}
