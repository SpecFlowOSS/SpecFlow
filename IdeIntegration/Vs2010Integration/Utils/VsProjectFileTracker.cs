using System;
using System.IO;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal class VsProjectFileTracker
    {
        private readonly Project project;
        private readonly bool followTrackingAfterRename;
        private string fileName;
        private readonly IVisualStudioTracer visualStudioTracer;
        private DateTime? LastChangeDate;

        public event Action<ProjectItem> FileChanged;

        public VsProjectFileTracker(Project project, string fileName, DteWithEvents dteWithEvents, IVisualStudioTracer visualStudioTracer, bool followTrackingAfterRename = false)
        {
            this.project = project;
            this.followTrackingAfterRename = followTrackingAfterRename;
            this.fileName = fileName;
            this.visualStudioTracer = visualStudioTracer;

            SetLastChangeDate(VsxHelper.FindProjectItemByProjectRelativePath(project, fileName));
            SubscribeToDteEvents(dteWithEvents);
        }

        private void SubscribeToDteEvents(DteWithEvents dteWithEvents)
        {
            dteWithEvents.ProjectItemsEvents.ItemAdded +=
                item =>
                {
                    visualStudioTracer.Trace("Item Added: " + item.Name, "VsProjectFileTracker");
                    if (IsItemRelevant(item))
                        OnFileChanged(item);
                };

            dteWithEvents.ProjectItemsEvents.ItemRemoved +=
                item =>
                {
                    visualStudioTracer.Trace("Item Removed: " + item.Name, "VsProjectFileTracker");
                    if (IsItemRelevant(item))
                        OnFileChanged(null);
                };

            dteWithEvents.ProjectItemsEvents.ItemRenamed +=
                (item, oldName) =>
                {
                    visualStudioTracer.Trace("Item Renamed to: " + item.Name + " from " + oldName, "VsProjectFileTracker");
                    if (IsItemRelevant(item))
                    {
                        OnFileChanged(item);
                    }
                    else if (IsItemRelevant(item, oldName))
                    {
                        if (followTrackingAfterRename)
                        {
                            fileName = VsxHelper.GetProjectRelativePath(item);
                            OnFileChanged(item);
                        }
                        else
                        {
                            OnFileChanged(null);
                        }
                    }
                };

            dteWithEvents.DocumentEvents.DocumentSaved +=
                document =>
                {
                    visualStudioTracer.Trace("Document Saved: " + document, "VsProjectFileTracker");
                    ProjectItem item = document.ProjectItem;
                    if (IsItemRelevant(item))
                        OnFileChanged(item);
                };

            dteWithEvents.BuildEvents.OnBuildDone +=
                (scope, action) =>
                    {
                        this.visualStudioTracer.Trace("Build Done.", "VsProjectFileTracker");
                        ProjectItem item = VsxHelper.FindProjectItemByProjectRelativePath(project, fileName);
                        var newChangeDate = GetLastChangeDate(item);
                        if (newChangeDate != LastChangeDate)
                            OnFileChanged(item);
                    };
        }

        private bool IsItemRelevant(ProjectItem projectItem, string itemName = null)
        {
            return projectItem != null && projectItem.ContainingProject == project && IsFileNameMatching(projectItem, itemName);
        }

        private bool IsFileNameMatching(ProjectItem projectItem, string itemName = null)
        {
            var projectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            if (itemName != null)
                projectRelativePath = Path.Combine(Path.GetDirectoryName(projectRelativePath) ?? "", itemName);

            return string.Equals(fileName, projectRelativePath, StringComparison.InvariantCultureIgnoreCase);
        }

        private void OnFileChanged(ProjectItem projectItem)
        {
            SetLastChangeDate(projectItem);

            visualStudioTracer.Trace("File Changed: " + fileName, "VsProjectFileTracker");
            if (FileChanged != null)
                FileChanged(projectItem);
        }

        private DateTime? GetLastChangeDate(ProjectItem projectItem)
        {
            string filePath;
            if (projectItem == null || !File.Exists(filePath = VsxHelper.GetFileName(projectItem)))
            {
                return null;
            }

            return File.GetLastWriteTime(filePath);
        }

        private void SetLastChangeDate(ProjectItem projectItem)
        {
            LastChangeDate = GetLastChangeDate(projectItem);
        }
    }
}