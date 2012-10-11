using System;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal sealed class VsProjectFileTracker : VsProjectFileTrackerBase
    {
        private readonly bool followTrackingAfterRename;
        private string fileName;
        private DateTime? LastChangeDate;

        public VsProjectFileTracker(ProjectItem projectItem, DteWithEvents dteWithEvents, IVisualStudioTracer visualStudioTracer, bool followTrackingAfterRename = false) :
            this(projectItem.ContainingProject, VsxHelper.GetProjectRelativePath(projectItem), dteWithEvents, visualStudioTracer, followTrackingAfterRename)
        {
            
        }

        public VsProjectFileTracker(Project project, string fileName, DteWithEvents dteWithEvents, IVisualStudioTracer tracer, bool followTrackingAfterRename = false) :
            base(project, dteWithEvents, tracer)
        {
            this.followTrackingAfterRename = followTrackingAfterRename;
            this.fileName = fileName;

            SetLastChangeDate(VsxHelper.FindProjectItemByProjectRelativePath(project, fileName));

            SubscribeToDteEvents();
        }

        protected override void SubscribeToDteEvents()
        {
            base.SubscribeToDteEvents();
            dteWithEvents.OnBuildDone += BuildEventsOnOnBuildDone;
        }

        protected override void UnsubscribeFromDteEvents()
        {
            base.UnsubscribeFromDteEvents();
            dteWithEvents.OnBuildDone -= BuildEventsOnOnBuildDone;
        }

        protected override void SetupListeningToFiles()
        {
            var item = GetProjectItem();
            if (item != null)
                StartListeningToFile(item);
        }

        private void BuildEventsOnOnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            this.tracer.Trace("Build Done.", "VsProjectFileTracker");
            ProjectItem item = VsxHelper.FindProjectItemByProjectRelativePath(project, fileName);
            var newChangeDate = VsxHelper.GetLastChangeDate(item);
            if (newChangeDate != LastChangeDate)
                OnFileChanged(item);
        }

        protected override void OnFileBecomesIrrelevant(ProjectItem item, string oldName)
        {
            if (followTrackingAfterRename)
            {
                fileName = VsxHelper.GetProjectRelativePath(item);
                OnFileRenamed(item, fileName);
            }
            else
            {
                OnFileOutOfScope(item, GetProjectRelativePathWithFileName(item, oldName));
            }
        }

        protected override bool IsFileNameMatching(ProjectItem projectItem, string itemName = null)
        {
            string projectRelativePath = GetProjectRelativePathWithFileName(projectItem, itemName);

            return string.Equals(fileName, projectRelativePath, StringComparison.InvariantCultureIgnoreCase);
        }

        private void SetLastChangeDate(ProjectItem projectItem)
        {
            LastChangeDate = VsxHelper.GetLastChangeDate(projectItem);
        }

        protected override void OnFileChanged(ProjectItem projectItem)
        {
            SetLastChangeDate(projectItem);
            tracer.Trace("File Changed: " + fileName, "VsProjectFileTracker");
            base.OnFileChanged(projectItem);
        }

        protected override void OnFileRenamed(ProjectItem projectItem, string oldProjectRelativeFileName)
        {
            SetLastChangeDate(projectItem);
            tracer.Trace("File renamed: " + fileName, "VsProjectFileTracker");
            base.OnFileRenamed(projectItem, oldProjectRelativeFileName);
        }

        public ProjectItem GetProjectItem()
        {
            return VsxHelper.FindProjectItemByProjectRelativePath(project, fileName);
        }
    }
}