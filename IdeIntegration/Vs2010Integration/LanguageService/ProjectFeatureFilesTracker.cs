using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using VSLangProj;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class FeatureFileInfo
    {
        public string ProjectRelativePath { get; private set; }
        public Version GeneratorVersion { get; set; }
        public Feature ParsedFeature { get; set; }

        public FeatureFileInfo(ProjectItem projectItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
        }

        public void Rename(string newProjectRelativePath)
        {
            ProjectRelativePath = newProjectRelativePath;
        }
    }

    public class ProjectFeatureFilesTracker : IDisposable
    {
        private VsProjectScope vsProjectScope;
        private List<FeatureFileInfo> featureFiles;
        private VsProjectFilesTracker filesTracker;

        public event Action Initialized;
        public event Action<FeatureFileInfo> FeatureFileRemoved;
        public event Action<FeatureFileInfo> FeatureFileUpdated;

        public IEnumerable<FeatureFileInfo> FeatureFiles { get { return featureFiles; } }

        public ProjectFeatureFilesTracker(VsProjectScope vsProjectScope)
        {
            this.vsProjectScope = vsProjectScope;

            featureFiles = GetFeatureFileProjectItems().Select(pi => new FeatureFileInfo(pi)).ToList();
            filesTracker = new VsProjectFilesTracker(this.vsProjectScope.Project, @"\.feature$", this.vsProjectScope.DteWithEvents, this.vsProjectScope.VisualStudioTracer);
            filesTracker.FileChanged += FilesTrackerOnFileChanged;
            filesTracker.FileRenamed += FilesTrackerOnFileRenamed;
            filesTracker.FileOutOfScope += FilesTrackerOnFileOutOfScope;
        }

        public void Run()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Initialize), DispatcherPriority.Background);
        }

        private void FilesTrackerOnFileOutOfScope(ProjectItem projectItem, string projectRelativePath)
        {
            FeatureFileInfo featureFileInfo = FindFeatureFileInfo(projectRelativePath);
            if (featureFileInfo != null)
            {
                RemoveFileInfo(featureFileInfo);
            }
        }

        private void FilesTrackerOnFileRenamed(ProjectItem projectItem, string oldName)
        {
            FeatureFileInfo featureFileInfo = FindFeatureFileInfo(oldName);
            if (featureFileInfo != null)
            {
                RenameFileInfo(featureFileInfo, VsxHelper.GetProjectRelativePath(projectItem));
            }
            else
            {
                AddFileInfo(projectItem);
            }
        }

        private void FilesTrackerOnFileChanged(ProjectItem projectItem)
        {
            FeatureFileInfo featureFileInfo = FindFeatureFileInfo(VsxHelper.GetProjectRelativePath(projectItem));
            if (featureFileInfo != null)
            {
                ChangeFile(featureFileInfo);
            }
            else
            {
                AddFileInfo(projectItem);
            }
        }

        private void AddFileInfo(ProjectItem projectItem)
        {
            var featureFileInfo = new FeatureFileInfo(projectItem);
            featureFiles.Add(featureFileInfo);
            Analyze(featureFileInfo);
        }

        private void ChangeFile(FeatureFileInfo featureFileInfo)
        {
            Analyze(featureFileInfo);
        }

        private void RemoveFileInfo(FeatureFileInfo featureFileInfo)
        {
            featureFiles.Remove(featureFileInfo);
            //remove from caches
            if (FeatureFileRemoved != null)
                FeatureFileRemoved(featureFileInfo);
        }

        private void RenameFileInfo(FeatureFileInfo featureFileInfo, string newPath)
        {
            featureFileInfo.Rename(newPath);
            // upadate caches?
        }

        private FeatureFileInfo FindFeatureFileInfo(string projectRelativePath)
        {
            return featureFiles.Find(ffi => string.Equals(ffi.ProjectRelativePath, projectRelativePath, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<ProjectItem> GetFeatureFileProjectItems()
        {
            return VsxHelper.GetAllPhysicalFileProjectItem(vsProjectScope.Project).Where(IsFeatureFileProjectItem);
        }

        internal static bool IsFeatureFileProjectItem(ProjectItem pi)
        {
            return ".feature".Equals(Path.GetExtension(pi.Name), StringComparison.InvariantCultureIgnoreCase);
        }

        private void Initialize()
        {
            featureFiles.ForEach(ffi => DoAnalyze(ffi, false));
            if (Initialized != null)
                Initialized();

            vsProjectScope.GherkinDialectServicesChanged += OnGherkinDialectServicesChanged;
        }

        private void OnGherkinDialectServicesChanged(object sender, EventArgs eventArgs)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => featureFiles.ForEach(ffi => DoAnalyze(ffi, true))), DispatcherPriority.Background);
        }

        private void Analyze(FeatureFileInfo featureFileInfo)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => DoAnalyze(featureFileInfo, true)), DispatcherPriority.Background);
        }

        private void DoAnalyze(FeatureFileInfo featureFileInfo, bool fireUpdatedEvent)
        {
            var pi = VsxHelper.FindProjectItemByProjectRelativePath(vsProjectScope.Project, featureFileInfo.ProjectRelativePath);
            if (pi == null)
            {
                RemoveFileInfo(featureFileInfo); // this must be a problem, but anyway
                return;
            }

            vsProjectScope.VisualStudioTracer.Trace("Analyzing feature file: " + featureFileInfo.ProjectRelativePath, "ProjectFeatureFilesTracker");
            AnalyzeCodeBehind(featureFileInfo, pi);

            var fileContent = VsxHelper.GetFileContent(pi);
            featureFileInfo.ParsedFeature = ParseGherkinFile(fileContent, featureFileInfo.ProjectRelativePath, vsProjectScope.GherkinDialectServices.DefaultLanguage);

            if (fireUpdatedEvent && FeatureFileUpdated != null)
                FeatureFileUpdated(featureFileInfo);
        }

        public Feature ParseGherkinFile(string fileContent, string sourceFileName, CultureInfo defaultLanguage)
        {
            try
            {
                SpecFlowLangParser specFlowLangParser = new SpecFlowLangParser(defaultLanguage);

                StringReader featureFileReader = new StringReader(fileContent);

                var feature = specFlowLangParser.Parse(featureFileReader, sourceFileName);

                return feature;
            }
            catch (Exception ex)
            {
                vsProjectScope.VisualStudioTracer.Trace("Invalid feature file: " + sourceFileName, "ProjectFeatureFilesTracker");
                return null;
            }
        }

        private void AnalyzeCodeBehind(FeatureFileInfo featureFileInfo, ProjectItem projectItem)
        {
            var codeBehindItem = GetCodeBehindItem(projectItem);
            if (codeBehindItem != null)
            {
                string codeBehind = VsxHelper.GetFileContent(codeBehindItem);
                featureFileInfo.GeneratorVersion = SpecFlowGenerator.GetGeneratedFileSpecFlowVersion(new StringReader(codeBehind));
            }
        }

        private ProjectItem GetCodeBehindItem(ProjectItem projectItem)
        {
            if (projectItem.ProjectItems == null)
                return null;

            return projectItem.ProjectItems.Cast<ProjectItem>().FirstOrDefault();
        }

        public void ReGenerateAll(Func<FeatureFileInfo,bool> predicate = null)
        {
            if (predicate == null)
            {
                featureFiles.ForEach(ReGenerate);
                return;
            }

            foreach (var featureFileInfo in featureFiles.Where(predicate))
                ReGenerate(featureFileInfo);
        }

        private void ReGenerate(FeatureFileInfo featureFileInfo)
        {
            var projectItem = VsxHelper.FindProjectItemByProjectRelativePath(vsProjectScope.Project, featureFileInfo.ProjectRelativePath);
            if (projectItem != null)
            {
                VSProjectItem vsProjectItem = projectItem.Object as VSProjectItem;
                if (vsProjectItem != null)
                    vsProjectItem.RunCustomTool();
            }
        }

        public void Dispose()
        {
            vsProjectScope.GherkinDialectServicesChanged -= OnGherkinDialectServicesChanged;

            filesTracker.FileChanged -= FilesTrackerOnFileChanged;
            filesTracker.FileRenamed -= FilesTrackerOnFileRenamed;
            filesTracker.FileOutOfScope -= FilesTrackerOnFileOutOfScope;
            filesTracker.Dispose();
        }
    }
}