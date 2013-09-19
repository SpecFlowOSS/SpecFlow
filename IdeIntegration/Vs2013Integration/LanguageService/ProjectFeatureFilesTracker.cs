using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using VSLangProj;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class FeatureFileInfo : FileInfo
    {
        public Version GeneratorVersion { get; set; }
        public Feature ParsedFeature { get; set; }

        public FeatureFileInfo(ProjectItem projectItem, ProjectItem codeBehindItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            var codeBehindItemChangeDate = VsxHelper.GetLastChangeDate(codeBehindItem) ?? DateTime.MinValue;
            var featureFileLastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
            LastChangeDate = featureFileLastChangeDate > codeBehindItemChangeDate ? featureFileLastChangeDate : codeBehindItemChangeDate;
        }
    }

    internal class ProjectFeatureFilesTracker : ProjectFilesTracker<FeatureFileInfo>, IDisposable
    {
        private readonly VsProjectFilesTracker filesTracker;
        private readonly Lazy<ITestGenerator> testGeneratorForCodeBehindVersionDetection;

        public ProjectFeatureFilesTracker(VsProjectScope vsProjectScope) : base(vsProjectScope)
        {
            filesTracker = CreateFilesTracker(this.vsProjectScope.Project, @"\.feature$");
            testGeneratorForCodeBehindVersionDetection = new Lazy<ITestGenerator>(() => vsProjectScope.GeneratorServices.CreateTestGeneratorOfIDE(), true);
        }

        protected override FeatureFileInfo CreateFileInfo(ProjectItem projectItem)
        {
            return new FeatureFileInfo(projectItem, GetCodeBehindItem(projectItem));
        }

        protected override bool IsMatchingProjectItem(ProjectItem projectItem)
        {
            return ".feature".Equals(Path.GetExtension(projectItem.Name), StringComparison.InvariantCultureIgnoreCase);
        }

        protected override void AnalyzeInitially()
        {
            base.AnalyzeInitially();
            vsProjectScope.GherkinDialectServicesChanged += OnGherkinDialectServicesChanged;
        }

        private void OnGherkinDialectServicesChanged()
        {
            AnalyzeFilesBackground();
        }

        protected override void Analyze(FeatureFileInfo featureFileInfo, ProjectItem projectItem)
        {
            vsProjectScope.Tracer.Trace("Analyzing feature file: " + featureFileInfo.ProjectRelativePath, "ProjectFeatureFilesTracker");
            var codeBehindChangeDate = AnalyzeCodeBehind(featureFileInfo, projectItem);

            var fileContent = VsxHelper.GetFileContent(projectItem, loadLastSaved: true);
            featureFileInfo.ParsedFeature = ParseGherkinFile(fileContent, featureFileInfo.ProjectRelativePath, vsProjectScope.GherkinDialectServices.DefaultLanguage);
            var featureLastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
            featureFileInfo.LastChangeDate = featureLastChangeDate > codeBehindChangeDate ? featureLastChangeDate : codeBehindChangeDate;
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
            catch (Exception)
            {
                vsProjectScope.Tracer.Trace("Invalid feature file: " + sourceFileName, "ProjectFeatureFilesTracker");
                return null;
            }
        }

        private DateTime AnalyzeCodeBehind(FeatureFileInfo featureFileInfo, ProjectItem projectItem)
        {
            var codeBehindItem = GetCodeBehindItem(projectItem);
            if (codeBehindItem != null)
            {
                string codeBehindContent = VsxHelper.GetFileContent(codeBehindItem);
                DetectGeneratedTestVersion(featureFileInfo, codeBehindContent);
                var lastChangeDate = VsxHelper.GetLastChangeDate(codeBehindItem) ?? DateTime.MinValue;
                return lastChangeDate;
            }

            return DateTime.MinValue;
        }

        private void DetectGeneratedTestVersion(FeatureFileInfo featureFileInfo, string codeBehindContent)
        {
            try
            {
                var testGenerator = testGeneratorForCodeBehindVersionDetection.Value;
                featureFileInfo.GeneratorVersion = testGenerator.DetectGeneratedTestVersion(
                    new FeatureFileInput(featureFileInfo.ProjectRelativePath)
                        {
                            GeneratedTestFileContent = codeBehindContent
                        });
            }
            catch(Exception ex)
            {
                // if there was an error during version detect, we skip discovering the version for this file.
                vsProjectScope.Tracer.Trace(
                    string.Format("Cannot detect generated test version for file: {0}, error: {1}", featureFileInfo.ProjectRelativePath, ex.Message), GetType().Name);
            }
        }

        private ProjectItem GetCodeBehindItem(ProjectItem projectItem)
        {
            try
            {
                if (projectItem.ProjectItems == null)
                    return null;

                return projectItem.ProjectItems.Cast<ProjectItem>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                vsProjectScope.Tracer.Trace("Cannot get code behind item: {0}", this, ex);
                return null;
            }
        }

        public void ReGenerateAll(Func<FeatureFileInfo,bool> predicate = null)
        {
            if (predicate == null)
            {
                foreach (var featureFileInfo in Files)
                    ReGenerate(featureFileInfo);
                return;
            }

            foreach (var featureFileInfo in Files.Where(predicate))
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
            DisposeFilesTracker(filesTracker);
        }

        protected override void SaveToStepMapInternal(StepMap stepMap)
        {
            stepMap.FeatureSteps = new List<FeatureSteps>();
            foreach (var featureFileInfo in Files.Where(f => f.ParsedFeature != null))
            {
                stepMap.FeatureSteps.Add(new FeatureSteps
                                             {
                                                 FileName = featureFileInfo.ProjectRelativePath, 
                                                 TimeStamp = featureFileInfo.LastChangeDate,
                                                 Feature = featureFileInfo.ParsedFeature,
                                                 GeneratorVersion = featureFileInfo.GeneratorVersion
                                             });
            }
        }

        protected override void LoadFromStepMapInternal(StepMap stepMap)
        {
            if (stepMap.FeatureSteps == null)
                return;

            foreach (var featureSteps in stepMap.FeatureSteps)
            {
                try{
                    var fileInfo = FindFileInfo(featureSteps.FileName);
                    if (fileInfo == null)
                        continue;

                    if (fileInfo.IsDirty(featureSteps.TimeStamp))
                        continue;

                    fileInfo.ParsedFeature = featureSteps.Feature;
                    fileInfo.GeneratorVersion = featureSteps.GeneratorVersion;

                    FireFileUpdated(fileInfo);
                    fileInfo.IsError = false;
                    fileInfo.IsAnalyzed = true;
                }
                catch(Exception ex)
                {
                    vsProjectScope.Tracer.Trace(string.Format("Feature steps load error for {0}: {1}", featureSteps.FileName, ex), GetType().Name);
                }
            }

            vsProjectScope.Tracer.Trace("Applied loaded fieature file steps", GetType().Name);
        }
    }
}