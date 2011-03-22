using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IFileInfo
    {
        bool IsAnalyzed { get; set; }
        string ProjectRelativePath { get; }
        void Rename(string newProjectRelativePath);
    }

    internal abstract class ProjectFilesTracker<TFileInfo> where TFileInfo : class, IFileInfo 
    {
        protected readonly VsProjectScope vsProjectScope;
        protected List<TFileInfo> files;

        public event Action Initialized;
        public event Action Ready;
        public event Action<TFileInfo> FileRemoved;
        public event Action<TFileInfo> FileUpdated;

        public IEnumerable<TFileInfo> Files { get { return files; } }
        public bool IsInitialized { get { return files != null; } }

        protected ProjectFilesTracker(VsProjectScope vsProjectScope)
        {
            if (vsProjectScope.GherkinProcessingScheduler == null)
                throw new ArgumentException("GherkinProcessingScheduler is null", "vsProjectScope");

            this.vsProjectScope = vsProjectScope;
        }

        private void FilesTrackerOnFileOutOfScope(ProjectItem projectItem, string projectRelativePath)
        {
            if (!IsInitialized)
                return;

            var fileInfo = FindFileInfo(projectRelativePath);
            if (fileInfo != null)
            {
                RemoveFileInfo(fileInfo);
            }
        }

        private void FilesTrackerOnFileRenamed(ProjectItem projectItem, string oldName)
        {
            if (!IsInitialized)
                return;

            var fileInfo = FindFileInfo(oldName);
            if (fileInfo != null)
            {
                RenameFileInfo(fileInfo, VsxHelper.GetProjectRelativePath(projectItem));
            }
            else
            {
                AddFileInfo(projectItem);
            }
        }

        private void FilesTrackerOnFileChanged(ProjectItem projectItem)
        {
            if (!IsInitialized)
                return;

            bool isInScope = IsMatchingProjectItem(projectItem);
            var fileInfo = FindFileInfo(VsxHelper.GetProjectRelativePath(projectItem));
            if (fileInfo != null)
            {
                if (isInScope)
                    ChangeFile(fileInfo);
                else
                    RemoveFileInfo(fileInfo);
            }
            else if (isInScope)
            {
                AddFileInfo(projectItem);
            }
        }

        private TFileInfo FindFileInfo(string projectRelativePath)
        {
            return Files.FirstOrDefault(ffi => string.Equals(ffi.ProjectRelativePath, projectRelativePath, StringComparison.InvariantCultureIgnoreCase));
        }

        protected VsProjectFilesTracker CreateFilesTracker(Project project, string regexPattern)
        {
            var result = new VsProjectFilesTracker(project, regexPattern, vsProjectScope.DteWithEvents, vsProjectScope.VisualStudioTracer);
            result.FileChanged += FilesTrackerOnFileChanged;
            result.FileRenamed += FilesTrackerOnFileRenamed;
            result.FileOutOfScope += FilesTrackerOnFileOutOfScope;
            return result;
        }

        protected void DisposeFilesTracker(VsProjectFilesTracker filesTracker)
        {
            filesTracker.FileChanged -= FilesTrackerOnFileChanged;
            filesTracker.FileRenamed -= FilesTrackerOnFileRenamed;
            filesTracker.FileOutOfScope -= FilesTrackerOnFileOutOfScope;
            filesTracker.Dispose();
        }

        private void DoTaskAsynch(Action action)
        {
            vsProjectScope.GherkinProcessingScheduler.EnqueueAnalyzingRequest(new DelegateTask(action));
        }

        public virtual void Initialize()
        {
            DoTaskAsynch(InitializeInternal);
        }

        public virtual void Run()
        {
            DoTaskAsynch(AnalyzeInitially);
        }

        protected virtual void InitializeInternal()
        {
            files = GetFileProjectItems().Select(CreateFileInfo).ToList();

            if (Initialized != null)
                Initialized();

            vsProjectScope.VisualStudioTracer.Trace("Initialized", GetType().Name);
        }

        protected virtual void AnalyzeInitially()
        {
            TFileInfo fileInfo;
            while ((fileInfo = files.FirstOrDefault(fi => !fi.IsAnalyzed)) != null)
            {
                AnalyzeInternal(fileInfo, true);
            }
            if (Ready != null)
                Ready();
        }

        protected void AnalyzeBackground(TFileInfo fileInfo)
        {
            DoTaskAsynch(() => AnalyzeInternal(fileInfo, true));
        }

        protected void AnalyzeFilesBackground()
        {
            DoTaskAsynch(() =>
                             {
                                 foreach (var fileInfo in Files)
                                 {
                                     AnalyzeInternal(fileInfo, true);
                                 }
                             });
        }

        private void AnalyzeInternal(TFileInfo fileInfo, bool fireUpdatedEvent)
        {
            var pi = FindProjectItemByProjectRelativePath(fileInfo);
            if (pi == null)
            {
                RemoveFileInfo(fileInfo); // this must be a problem, but anyway
                return;
            }

            try
            {
                Analyze(fileInfo, pi);

                if (fireUpdatedEvent && FileUpdated != null)
                    FileUpdated(fileInfo);
            }
            catch(Exception)
            {
                
            }
            finally
            {
                fileInfo.IsAnalyzed = true;
            }
        }

        private ProjectItem FindProjectItemByProjectRelativePath(TFileInfo fileInfo)
        {
            try
            {
                return GetProjects().Select(project => VsxHelper.FindProjectItemByProjectRelativePath(project, fileInfo.ProjectRelativePath)).FirstOrDefault(pi => pi != null);
            }
            catch(Exception)
            {
                return null;
            }
        }

        protected void OnFileRemoved(TFileInfo fileInfo)
        {
            if (FileRemoved != null)
                FileRemoved(fileInfo);
        }

        protected abstract void Analyze(TFileInfo fileInfo, ProjectItem projectItem);
        protected abstract TFileInfo CreateFileInfo(ProjectItem projectItem);
        protected virtual IEnumerable<ProjectItem> GetFileProjectItems()
        {
            return GetProjects().SelectMany(VsxHelper.GetAllPhysicalFileProjectItem).Where(IsMatchingProjectItem);
        }
        protected virtual IEnumerable<Project> GetProjects()
        {
            return new[] {vsProjectScope.Project};
        }
        protected abstract bool IsMatchingProjectItem(ProjectItem projectItem);


        protected virtual void AddFileInfo(ProjectItem projectItem)
        {
            var featureFileInfo = CreateFileInfo(projectItem);
            files.Add(featureFileInfo);
            AnalyzeBackground(featureFileInfo);
        }

        protected virtual void ChangeFile(TFileInfo featureFileInfo)
        {
            AnalyzeBackground(featureFileInfo);
        }

        protected virtual void RemoveFileInfo(TFileInfo featureFileInfo)
        {
            files.Remove(featureFileInfo);
            //remove from caches
            OnFileRemoved(featureFileInfo);
        }

        protected virtual void RenameFileInfo(TFileInfo featureFileInfo, string newPath)
        {
            featureFileInfo.Rename(newPath);
            // upadate caches?
        }

    }
}