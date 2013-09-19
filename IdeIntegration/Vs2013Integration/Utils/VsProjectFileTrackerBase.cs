using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal abstract class VsProjectFileTrackerBase : IDisposable
    {
        protected Project project;
        protected DteWithEvents dteWithEvents;
        protected IIdeTracer tracer;
        private readonly Timer timer;
        private HashSet<string> filesChangedOnDisk = new HashSet<string>();

        public event Action<ProjectItem> FileChanged;
        public event Action<ProjectItem, string> FileOutOfScope;
        public event Action<ProjectItem, string> FileRenamed;

        protected VsProjectFileTrackerBase(Project project, DteWithEvents dteWithEvents, IIdeTracer tracer)
        {
            this.project = project;
            this.dteWithEvents = dteWithEvents;
            this.tracer = tracer;

            this.timer = new Timer(HandleFilesChangedOnDisk, null, Timeout.Infinite, Timeout.Infinite);
        }

        protected virtual void SubscribeToDteEvents()
        {
            dteWithEvents.ProjectItemsEvents.ItemAdded += ProjectItemsEventsOnItemAdded;
            dteWithEvents.ProjectItemsEvents.ItemRemoved += ProjectItemsEventsOnItemRemoved;
            dteWithEvents.ProjectItemsEvents.ItemRenamed += ProjectItemsEventsOnItemRenamed;
            dteWithEvents.DocumentEvents.DocumentSaved += DocumentEventsOnDocumentSaved;
            dteWithEvents.FileChangeEventsListener.FileChanged += FileChangedOnDisk;

            SetupListeningToFiles();
        }

        protected abstract void SetupListeningToFiles();

        protected virtual void UnsubscribeFromDteEvents()
        {
            dteWithEvents.ProjectItemsEvents.ItemAdded -= ProjectItemsEventsOnItemAdded;
            dteWithEvents.ProjectItemsEvents.ItemRemoved -= ProjectItemsEventsOnItemRemoved;
            dteWithEvents.ProjectItemsEvents.ItemRenamed -= ProjectItemsEventsOnItemRenamed;
            dteWithEvents.DocumentEvents.DocumentSaved -= DocumentEventsOnDocumentSaved;
            dteWithEvents.FileChangeEventsListener.FileChanged -= FileChangedOnDisk;

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            filesChangedOnDisk.Clear();
        }

        private void ProjectItemsEventsOnItemAdded(ProjectItem item)
        {
            tracer.Trace("Item Added: " + item.Name, "VsProjectFileTracker");
            if (IsItemRelevant(item))
                OnFileChanged(item);

            StartListeningToFile(item);
        }

        protected void StartListeningToFile(ProjectItem item)
        {
            var file = VsxHelper.GetFileName(item);
            dteWithEvents.FileChangeEventsListener.StartListeningToFile(file);
        }

        private void ProjectItemsEventsOnItemRemoved(ProjectItem item)
        {
            tracer.Trace("Item Removed: " + item.Name, "VsProjectFileTracker");
            if (IsItemRelevant(item))
                OnFileOutOfScope(item, VsxHelper.GetProjectRelativePath(item));
        }

        private void ProjectItemsEventsOnItemRenamed(ProjectItem item, string oldName)
        {
            tracer.Trace("Item Renamed to: " + item.Name + " from " + oldName, "VsProjectFileTracker");
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
            ProjectItem item = VsxHelper.TryGetProjectItem(document);
            if (item == null || !IsItemRelevant(item))
                return;

            tracer.Trace("Document Saved: {0}", this, VsxHelper.GetFileName(document.ProjectItem));

            // if the file was saved throgh VS, we remove from the processing of "outside of vs" change handling
            filesChangedOnDisk.Remove(VsxHelper.GetFileName(item));

            if (IsItemRelevant(item))
                OnFileChanged(item);
        }

        private void QueueHandlingFileOnDiskChange(string filePath)
        {
            const int FILE_CHANGE_DELAY_MSEC = 2000;

            filesChangedOnDisk.Add(filePath);
            timer.Change(FILE_CHANGE_DELAY_MSEC, Timeout.Infinite);
        }

        private void FileChangedOnDisk(string filePath)
        {
            var item = VsxHelper.FindProjectItemByFilePath(project, filePath);
            if (item == null || !IsItemRelevant(item))
                return;

            tracer.Trace("File change on disk handling queued: {0}", this, filePath);
            QueueHandlingFileOnDiskChange(filePath);
        }

        private void HandleFilesChangedOnDisk(object _)
        {
            if (filesChangedOnDisk.Count == 0)
                return;
            var filesChanged = filesChangedOnDisk;
            filesChangedOnDisk = new HashSet<string>();

            foreach (var filePath in filesChanged)
            {
                try
                {
                    var item = VsxHelper.FindProjectItemByFilePath(project, filePath);
                    if (item == null)
                        return;

                    // if the file is open, we have to wait until VS reloads the file content, because
                    // until it is not reloaded, the file code model might be out of sync with the new content.
                    if (item.IsOpen[Constants.vsViewKindAny])
                    {
                        string contentOnDisk = VsxHelper.GetFileContent(item, loadLastSaved: true);
                        string contentInVS = VsxHelper.GetFileContent(item, loadLastSaved: false);

                        if (!contentOnDisk.Equals(contentInVS))
                        {
                            tracer.Trace("File is open and not in sync, reschedule update: {0}", this, filePath);
                            QueueHandlingFileOnDiskChange(filePath);
                            continue;
                        }
                    }

                    tracer.Trace("File changed outside of Visual Studio: {0}", this, filePath);
                    if (IsItemRelevant(item))
                        OnFileChanged(item);
                }
                catch(Exception ex)
                {
                    tracer.Trace("Error during file change handling: {0}", this, ex);
                }
            }
        }

        protected bool IsItemRelevant(ProjectItem projectItem, string itemName = null)
        {
            return projectItem != null && projectItem.ContainingProject.UniqueName.Equals(project.UniqueName) && IsFileNameMatching(projectItem, itemName);
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

            StartListeningToFile(projectItem);
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
}