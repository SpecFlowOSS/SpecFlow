using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using VSLangProj;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class FeatureFileInfo : IFileInfo
    {
        public string ProjectRelativePath { get; private set; }
        public bool IsAnalyzed { get; set; }
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

    internal class ProjectFeatureFilesTracker : ProjectFilesTracker<FeatureFileInfo>, IDisposable
    {
        private readonly VsProjectFilesTracker filesTracker;

        public ProjectFeatureFilesTracker(VsProjectScope vsProjectScope) : base(vsProjectScope)
        {
            filesTracker = CreateFilesTracker(this.vsProjectScope.Project, @"\.feature$");
        }

        protected override FeatureFileInfo CreateFileInfo(ProjectItem projectItem)
        {
            return new FeatureFileInfo(projectItem);
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

        private void OnGherkinDialectServicesChanged(object sender, EventArgs eventArgs)
        {
            AnalyzeFilesBackground();
        }

        protected override void Analyze(FeatureFileInfo featureFileInfo, ProjectItem projectItem)
        {
            vsProjectScope.VisualStudioTracer.Trace("Analyzing feature file: " + featureFileInfo.ProjectRelativePath, "ProjectFeatureFilesTracker");
            AnalyzeCodeBehind(featureFileInfo, projectItem);

            var fileContent = VsxHelper.GetFileContent(projectItem, loadLastSaved: true);
            featureFileInfo.ParsedFeature = ParseGherkinFile(fileContent, featureFileInfo.ProjectRelativePath, vsProjectScope.GherkinDialectServices.DefaultLanguage);
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
                using (var testGenerator = vsProjectScope.GeneratorServices.CreateTestGenerator())
                {
                    featureFileInfo.GeneratorVersion = testGenerator.DetectGeneratedTestVersion(
                        new FeatureFileInput(featureFileInfo.ProjectRelativePath)
                            {
                                GeneratedTestFileContent = codeBehind
                            });
                }
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
    }
}