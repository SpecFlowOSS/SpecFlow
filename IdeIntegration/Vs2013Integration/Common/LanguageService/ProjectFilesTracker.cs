using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public abstract class FileInfo
    {
        public bool IsAnalyzed { get; set; }
        public bool IsError { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string ProjectRelativePath { get; set; }

        public virtual void Rename(string newProjectRelativePath)
        {
            ProjectRelativePath = newProjectRelativePath;
        }

        public bool IsDirty(DateTime timeStamp)
        {
            return LastChangeDate > timeStamp.AddMilliseconds(0.5);
        }
    }

    internal abstract class ProjectFilesTracker<TFileInfo> where TFileInfo : FileInfo 
    {
        protected readonly VsProjectScope vsProjectScope;
        protected List<TFileInfo> files;

        public event Action Initialized;
        public event Action Ready;
        public event Action<TFileInfo> FileRemoved;
        public event Action<TFileInfo> FileUpdated;

        public IEnumerable<TFileInfo> Files { get { return files; } }
        public bool IsInitialized { get { return files != null; } }
        public bool IsStepMapDirty { get; private set; }

        protected ProjectFilesTracker(VsProjectScope vsProjectScope)
        {
            if (vsProjectScope.GherkinProcessingScheduler == null)
                throw new ArgumentException("GherkinProcessingScheduler is null", "vsProjectScope");

            this.vsProjectScope = vsProjectScope;
            this.IsStepMapDirty = true;
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

        protected TFileInfo FindFileInfo(string projectRelativePath)
        {
            return Files.FirstOrDefault(ffi => string.Equals(ffi.ProjectRelativePath, projectRelativePath, StringComparison.InvariantCultureIgnoreCase));
        }

        protected VsProjectFilesTracker CreateFilesTracker(Project project, string regexPattern)
        {
            var result = new VsProjectFilesTracker(project, regexPattern, vsProjectScope.DteWithEvents, vsProjectScope.Tracer);
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

        protected void DoTaskAsynch(Action action, string taskName)
        {
            vsProjectScope.GherkinProcessingScheduler.EnqueueAnalyzingRequest(new DelegateTask(action, taskName));
        }

        public virtual void Initialize()
        {
            DoTaskAsynch(InitializeInternally, "Initialize " + GetType().Name);
        }

        public virtual void Run()
        {
            DoTaskAsynch(RunInternal, string.Format("Analyze all starting ({0})", GetType().Name));
        }

        private void RunInternal()
        {
            foreach (var fileInfo in Files.Where(fi => !fi.IsAnalyzed).ToArray())
            {
                var fi = fileInfo;
                DoTaskAsynch(() => AnalyzeInternal(fi, true), string.Format("Analyze {1} ({0})", GetType().Name, fi.ProjectRelativePath));
            }
            DoTaskAsynch(AnalyzeInitially, string.Format("Analyze all finishing ({0})", GetType().Name));
        }

        protected virtual void InitializeInternally()
        {
            InitializeInternal();

            if (Initialized != null)
                Initialized();

            vsProjectScope.Tracer.Trace("Initialized", GetType().Name);
        }

        protected virtual void InitializeInternal()
        {
            files = GetFileProjectItems().Select(CreateFileInfo).ToList();
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
            DoTaskAsynch(() => AnalyzeInternal(fileInfo, true), string.Format("Analyze {1} ({0})", GetType().Name, fileInfo.ProjectRelativePath));
        }

        protected void AnalyzeFilesBackground()
        {
            DoTaskAsynch(() =>
                             {
                                 foreach (var fileInfo in Files)
                                 {
                                     AnalyzeInternal(fileInfo, true);
                                 }
                             }, 
                             string.Format("Re-analyze all ({0})", GetType().Name));
        }

        protected virtual bool HandleMissingProjectItem(TFileInfo fileInfo)
        {
            RemoveFileInfo(fileInfo); // this must be a problem, but anyway
            return true;
        }

        private void AnalyzeInternal(TFileInfo fileInfo, bool fireUpdatedEvent)
        {
            var pi = FindProjectItemByProjectRelativePath(fileInfo);
            if (pi == null)
            {
                if (HandleMissingProjectItem(fileInfo))
                    return;
            }

            try
            {
                Analyze(fileInfo, pi);
                fileInfo.IsError = false;

                if (fireUpdatedEvent)
                    FireFileUpdated(fileInfo);
            }
            catch(Exception exception)
            {
                fileInfo.IsError = true;
                vsProjectScope.Tracer.Trace("Exception: " + exception.ToString(), GetType().Name);
            }
            finally
            {
                IsStepMapDirty = true;
                fileInfo.IsAnalyzed = true;
            }
        }

        protected void FireFileUpdated(TFileInfo fileInfo)
        {
            if (FileUpdated != null)
                FileUpdated(fileInfo);
        }

        private ProjectItem FindProjectItemByProjectRelativePath(TFileInfo fileInfo)
        {
            try
            {
                return GetProjects().Select(project => VsxHelper.FindProjectItemByProjectRelativePath(project, fileInfo.ProjectRelativePath)).FirstOrDefault(pi => pi != null);
            }
            catch(Exception exception)
            {
                vsProjectScope.Tracer.Trace("Exception: " + exception.ToString(), GetType().Name);
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
            var fileInfo = CreateFileInfo(projectItem);
            files.Add(fileInfo);
            AnalyzeBackground(fileInfo);
        }

        protected void AddFileInfo(TFileInfo fileInfo)
        {
            files.Add(fileInfo);
            AnalyzeBackground(fileInfo);
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

        public void LoadFromStepMap(StepMap stepMap)
        {
            DoTaskAsynch(() =>
                             {
                                 LoadFromStepMapInternal(stepMap);
                                 IsStepMapDirty = false;
                             }, 
                             string.Format("Load from step map ({0})", GetType().Name));
        }

        public void SaveToStepMap(StepMap stepMap)
        {
            SaveToStepMapInternal(stepMap);
            IsStepMapDirty = false;
        }

        protected abstract void LoadFromStepMapInternal(StepMap stepMap);
        protected abstract void SaveToStepMapInternal(StepMap stepMap);
    }
}