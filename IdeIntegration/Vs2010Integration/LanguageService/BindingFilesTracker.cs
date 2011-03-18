using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
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
        private readonly Dictionary<BindingAssemblyInfo, VsProjectFilesTracker> filesTracker;

        public BindingFilesTracker(VsProjectScope vsProjectScope) : base(vsProjectScope)
        {
            var mainProject = vsProjectScope.Project;
            var bindingAssemblies = Enumerable.Empty<BindingAssemblyInfo>()
                .Append(new BindingAssemblyInfo(mainProject))
                .Concat(vsProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.AdditionalStepAssemblies.Select(
                        assembly => new BindingAssemblyInfo(assembly, mainProject.DTE)).Where(ai => ai.Project != mainProject));

            filesTracker = bindingAssemblies.ToDictionary(
                ai => ai,
                ai => ai.IsProject ? CreateFilesTracker(ai) : null);
        }

        private VsProjectFilesTracker CreateFilesTracker(BindingAssemblyInfo ai)
        {
            return CreateFilesTracker(ai.Project, @"\.(cs|vb)$");
        }

        protected override void Analyze(BindingFileInfo fileInfo, ProjectItem projectItem)
        {
            //TODO
        }

        protected override BindingFileInfo CreateFileInfo(ProjectItem projectItem)
        {
            return new BindingFileInfo(projectItem);
        }

        protected override IEnumerable<ProjectItem> GetFileProjectItems()
        {
            return VsxHelper.GetAllPhysicalFileProjectItem(vsProjectScope.Project).Where(IsFeatureFileProjectItem);
        }

        internal static bool IsFeatureFileProjectItem(ProjectItem pi)
        {
            var extension = Path.GetExtension(pi.Name);
            return
                ".cs".Equals(extension, StringComparison.InvariantCultureIgnoreCase) ||
                ".vb".Equals(extension, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Dispose()
        {
            foreach (var vsProjectFilesTracker in filesTracker.Values.Where(t => t != null))
                DisposeFilesTracker(vsProjectFilesTracker);
            filesTracker.Clear();
        }
    }
}
