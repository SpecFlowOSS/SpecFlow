using System;
using System.IO;
using System.Text.RegularExpressions;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal abstract class VsProjectFileTrackerBase : IDisposable
    {
        protected Project project;
        protected DteWithEvents dteWithEvents;
        protected IVisualStudioTracer visualStudioTracer;

        public event Action<ProjectItem> FileChanged;
        public event Action<ProjectItem, string> FileOutOfScope;
        public event Action<ProjectItem, string> FileRenamed;

        protected VsProjectFileTrackerBase(Project project, DteWithEvents dteWithEvents, IVisualStudioTracer visualStudioTracer)
        {
            this.project = project;
            this.dteWithEvents = dteWithEvents;
            this.visualStudioTracer = visualStudioTracer;
        }

        protected virtual void SubscribeToDteEvents()
        {
            dteWithEvents.ProjectItemsEvents.ItemAdded += ProjectItemsEventsOnItemAdded;
            dteWithEvents.ProjectItemsEvents.ItemRemoved += ProjectItemsEventsOnItemRemoved;
            dteWithEvents.ProjectItemsEvents.ItemRenamed += ProjectItemsEventsOnItemRenamed;
            dteWithEvents.DocumentEvents.DocumentSaved += DocumentEventsOnDocumentSaved;
        }

        protected virtual void UnsubscribeFromDteEvents()
        {
            dteWithEvents.ProjectItemsEvents.ItemAdded -= ProjectItemsEventsOnItemAdded;
            dteWithEvents.ProjectItemsEvents.ItemRemoved -= ProjectItemsEventsOnItemRemoved;
            dteWithEvents.ProjectItemsEvents.ItemRenamed -= ProjectItemsEventsOnItemRenamed;
            dteWithEvents.DocumentEvents.DocumentSaved -= DocumentEventsOnDocumentSaved;
        }

        private void ProjectItemsEventsOnItemAdded(ProjectItem item)
        {
            visualStudioTracer.Trace("Item Added: " + item.Name, "VsProjectFileTracker");
            if (IsItemRelevant(item))
                OnFileChanged(item);
        }

        private void ProjectItemsEventsOnItemRemoved(ProjectItem item)
        {
            visualStudioTracer.Trace("Item Removed: " + item.Name, "VsProjectFileTracker");
            if (IsItemRelevant(item))
                OnFileOutOfScope(item, VsxHelper.GetProjectRelativePath(item));
        }

        private void ProjectItemsEventsOnItemRenamed(ProjectItem item, string oldName)
        {
            visualStudioTracer.Trace("Item Renamed to: " + item.Name + " from " + oldName, "VsProjectFileTracker");
            if (IsItemRelevant(item))
            {
                OnFileRenamed(item, oldName);
            }
            else if (IsItemRelevant(item, oldName))
            {
                OnFileBecomesIrrelevant(item, oldName);
            }
        }

        protected abstract void OnFileBecomesIrrelevant(ProjectItem item, string oldName);

        private void DocumentEventsOnDocumentSaved(Document document)
        {
            visualStudioTracer.Trace("Document Saved: " + document, "VsProjectFileTracker");
            ProjectItem item = document.ProjectItem;
            if (IsItemRelevant(item))
                OnFileChanged(item);
        }

        private bool IsItemRelevant(ProjectItem projectItem, string itemName = null)
        {
            return projectItem != null && projectItem.ContainingProject == project && IsFileNameMatching(projectItem, itemName);
        }

        protected abstract bool IsFileNameMatching(ProjectItem projectItem, string itemName = null);

        protected virtual void OnFileChanged(ProjectItem projectItem)
        {
            if (FileChanged != null)
                FileChanged(projectItem);
        }

        protected virtual void OnFileOutOfScope(ProjectItem projectItem, string projectRelativeFileName)
        {
            if (FileOutOfScope != null)
                FileOutOfScope(projectItem, projectRelativeFileName);
        }

        protected virtual void OnFileRenamed(ProjectItem projectItem, string oldProjectRelativeFileName)
        {
            if (FileRenamed != null)
                FileRenamed(projectItem, oldProjectRelativeFileName);
        }

        protected string GetProjectRelativePathWithFileName(ProjectItem projectItem, string itemName)
        {
            var projectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            if (itemName != null)
                projectRelativePath = Path.Combine(Path.GetDirectoryName(projectRelativePath) ?? "", itemName);
            return projectRelativePath;
        }

        public void Dispose()
        {
            UnsubscribeFromDteEvents();
        }
    }

    internal sealed class VsProjectFilesTracker : VsProjectFileTrackerBase
    {
        private readonly Regex fileNameRe;

        public VsProjectFilesTracker(Project project, string regexPattern, DteWithEvents dteWithEvents, IVisualStudioTracer visualStudioTracer) : base(project, dteWithEvents, visualStudioTracer)
        {
            fileNameRe = new Regex(regexPattern, RegexOptions.IgnoreCase);

            SubscribeToDteEvents();
        }

        protected override void OnFileBecomesIrrelevant(ProjectItem item, string oldName)
        {
            OnFileOutOfScope(item, GetProjectRelativePathWithFileName(item, oldName));
        }

        protected override bool IsFileNameMatching(ProjectItem projectItem, string itemName = null)
        {
            var projectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            if (itemName != null)
                projectRelativePath = Path.Combine(Path.GetDirectoryName(projectRelativePath) ?? "", itemName);

            return fileNameRe.Match(projectRelativePath).Success;
        }
    }

    internal sealed class VsProjectFileTracker : VsProjectFileTrackerBase
    {
        private readonly bool followTrackingAfterRename;
        private string fileName;
        private DateTime? LastChangeDate;

        public VsProjectFileTracker(ProjectItem projectItem, DteWithEvents dteWithEvents, IVisualStudioTracer visualStudioTracer, bool followTrackingAfterRename = false) :
            this(projectItem.ContainingProject, VsxHelper.GetProjectRelativePath(projectItem), dteWithEvents, visualStudioTracer, followTrackingAfterRename)
        {
            
        }

        public VsProjectFileTracker(Project project, string fileName, DteWithEvents dteWithEvents, IVisualStudioTracer visualStudioTracer, bool followTrackingAfterRename = false) :
            base(project, dteWithEvents, visualStudioTracer)
        {
            this.followTrackingAfterRename = followTrackingAfterRename;
            this.fileName = fileName;

            SetLastChangeDate(VsxHelper.FindProjectItemByProjectRelativePath(project, fileName));

            SubscribeToDteEvents();
        }

        protected override void SubscribeToDteEvents()
        {
            base.SubscribeToDteEvents();
            dteWithEvents.BuildEvents.OnBuildDone += BuildEventsOnOnBuildDone;
        }

        protected override void UnsubscribeFromDteEvents()
        {
            base.UnsubscribeFromDteEvents();
            dteWithEvents.BuildEvents.OnBuildDone += BuildEventsOnOnBuildDone;
        }

        private void BuildEventsOnOnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            this.visualStudioTracer.Trace("Build Done.", "VsProjectFileTracker");
            ProjectItem item = VsxHelper.FindProjectItemByProjectRelativePath(project, fileName);
            var newChangeDate = GetLastChangeDate(item);
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

        protected override void OnFileChanged(ProjectItem projectItem)
        {
            SetLastChangeDate(projectItem);
            visualStudioTracer.Trace("File Changed: " + fileName, "VsProjectFileTracker");
            base.OnFileChanged(projectItem);
        }

        protected override void OnFileRenamed(ProjectItem projectItem, string oldProjectRelativeFileName)
        {
            SetLastChangeDate(projectItem);
            visualStudioTracer.Trace("File renamed: " + fileName, "VsProjectFileTracker");
            base.OnFileRenamed(projectItem, oldProjectRelativeFileName);
        }

        public ProjectItem GetProjectItem()
        {
            return VsxHelper.FindProjectItemByProjectRelativePath(project, fileName);
        }
    }
}