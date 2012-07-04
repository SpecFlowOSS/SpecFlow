using System.IO;
using System.Text.RegularExpressions;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal sealed class VsProjectFilesTracker : VsProjectFileTrackerBase
    {
        private readonly Regex fileNameRe;

        public VsProjectFilesTracker(Project project, string regexPattern, DteWithEvents dteWithEvents, IIdeTracer tracer) : base(project, dteWithEvents, tracer)
        {
            fileNameRe = new Regex(regexPattern, RegexOptions.IgnoreCase);

            SubscribeToDteEvents();
        }

        protected override void SetupListeningToFiles()
        {
            foreach (var projectItem in VsxHelper.GetAllPhysicalFileProjectItem(project))
            {
                if (IsItemRelevant(projectItem))
                    StartListeningToFile(projectItem);
            }
        }

        protected override void OnFileBecomesIrrelevant(ProjectItem item, string oldName)
        {
            OnFileOutOfScope(item, GetProjectRelativePathWithFileName(item, oldName));
        }

        protected override bool IsFileNameMatching(ProjectItem projectItem, string itemName = null)
        {
            var projectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            if (projectRelativePath == null)
                return false;

            if (itemName != null)
                projectRelativePath = Path.Combine(Path.GetDirectoryName(projectRelativePath) ?? "", itemName);

            return fileNameRe.Match(projectRelativePath).Success;
        }
    }
}