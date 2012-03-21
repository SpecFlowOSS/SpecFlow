using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    public class TestGenerator : ErrorHandlingTestGenerator, ITestGenerator
    {
        protected readonly GeneratorConfiguration generatorConfiguration;
        protected readonly ProjectSettings projectSettings;
        protected readonly ITestHeaderWriter testHeaderWriter;
        protected readonly ITestUpToDateChecker testUpToDateChecker;
        private readonly IFeatureGeneratorRegistry featureGeneratorRegistry;
        protected readonly CodeDomHelper codeDomHelper;

        public TestGenerator(GeneratorConfiguration generatorConfiguration, ProjectSettings projectSettings, ITestHeaderWriter testHeaderWriter, ITestUpToDateChecker testUpToDateChecker, IFeatureGeneratorRegistry featureGeneratorRegistry, CodeDomHelper codeDomHelper)
        {
            if (generatorConfiguration == null) throw new ArgumentNullException("generatorConfiguration");
            if (projectSettings == null) throw new ArgumentNullException("projectSettings");
            if (testHeaderWriter == null) throw new ArgumentNullException("testHeaderWriter");
            if (testUpToDateChecker == null) throw new ArgumentNullException("testUpToDateChecker");
            if (featureGeneratorRegistry == null) throw new ArgumentNullException("featureGeneratorRegistry");

            this.generatorConfiguration = generatorConfiguration;
            this.testUpToDateChecker = testUpToDateChecker;
            this.featureGeneratorRegistry = featureGeneratorRegistry;
            this.codeDomHelper = codeDomHelper;
            this.testHeaderWriter = testHeaderWriter;
            this.projectSettings = projectSettings;
        }

        protected override TestGeneratorResult GenerateTestFileWithExceptions(FeatureFileInput featureFileInput, GenerationSettings settings)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");
            if (settings == null) throw new ArgumentNullException("settings");

            var generatedTestFullPath = GetTestFullPath(featureFileInput);
            bool? preliminaryUpToDateCheckResult = null;
            if (settings.CheckUpToDate)
            {
                preliminaryUpToDateCheckResult = testUpToDateChecker.IsUpToDatePreliminary(featureFileInput, generatedTestFullPath, settings.UpToDateCheckingMethod);
                if (preliminaryUpToDateCheckResult == true)
                    return new TestGeneratorResult(null, true);
            }

            string generatedTestCode = GetGeneratedTestCode(featureFileInput);

            if (settings.CheckUpToDate && preliminaryUpToDateCheckResult != false)
            {
                var isUpToDate = testUpToDateChecker.IsUpToDate(featureFileInput, generatedTestFullPath, generatedTestCode, settings.UpToDateCheckingMethod);
                if (isUpToDate)
                    return new TestGeneratorResult(null, true);
            }

            if (settings.WriteResultToFile)
            {
                File.WriteAllText(generatedTestFullPath, generatedTestCode, Encoding.UTF8);
            }

            return new TestGeneratorResult(generatedTestCode, false);
        }

        private string GetGeneratedTestCode(FeatureFileInput featureFileInput)
        {
            using (var outputWriter = new IndentProcessingWriter(new StringWriter()))
            {
                var codeProvider = codeDomHelper.CreateCodeDomProvider();
                var codeNamespace = GenerateTestFileCode(featureFileInput);
                var options = new CodeGeneratorOptions
                                  {
                                      BracingStyle = "C"
                                  };

                AddSpecFlowHeader(codeProvider, outputWriter);
                codeProvider.GenerateCodeFromNamespace(codeNamespace, outputWriter, options);
                AddSpecFlowFooter(codeProvider, outputWriter);

                outputWriter.Flush();
                return outputWriter.ToString();
            }
        }

        private CodeNamespace GenerateTestFileCode(FeatureFileInput featureFileInput)
        {
            string targetNamespace = GetTargetNamespace(featureFileInput) ?? "SpecFlow.GeneratedTests";

            SpecFlowLangParser parser = new SpecFlowLangParser(generatorConfiguration.FeatureLanguage);
            Feature feature;
            using (var contentReader = featureFileInput.GetFeatureFileContentReader(projectSettings))
            {
                feature = parser.Parse(contentReader, featureFileInput.GetFullPath(projectSettings));
            }

            var featureGenerator = featureGeneratorRegistry.CreateGenerator(feature);

            var codeNamespace = featureGenerator.GenerateUnitTestFixture(feature, null, targetNamespace);
            return codeNamespace;
        }

        private string GetTargetNamespace(FeatureFileInput featureFileInput)
        {
            if (!string.IsNullOrEmpty(featureFileInput.CustomNamespace))
                return featureFileInput.CustomNamespace;

            if (projectSettings == null || string.IsNullOrEmpty(projectSettings.DefaultNamespace))
                return null;

            string targetNamespace = projectSettings.DefaultNamespace;

            var directoryName = Path.GetDirectoryName(featureFileInput.ProjectRelativePath);
            string namespaceExtension = string.IsNullOrEmpty(directoryName) ? null :
                string.Join(".", directoryName.TrimStart('\\', '/', '.').Split('\\', '/').Select(f => f.ToIdentifier()).ToArray());
            if (!string.IsNullOrEmpty(namespaceExtension))
                targetNamespace += "." + namespaceExtension;
            return targetNamespace;
        }

        public string GetTestFullPath(FeatureFileInput featureFileInput)
        {
            var path = featureFileInput.GetGeneratedTestFullPath(projectSettings);
            if (path != null)
                return path;

            return featureFileInput.GetFullPath(projectSettings) + GenerationTargetLanguage.GetExtension(projectSettings.ProjectPlatformSettings.Language);
        }

        #region Header & Footer
        protected override Version DetectGeneratedTestVersionWithExceptions(FeatureFileInput featureFileInput)
        {
            var generatedTestFullPath = GetTestFullPath(featureFileInput);
            return testHeaderWriter.DetectGeneratedTestVersion(featureFileInput.GetGeneratedTestContent(generatedTestFullPath));
        }

        private void AddSpecFlowHeader(CodeDomProvider codeProvider, TextWriter outputWriter)
        {
            const string specFlowHeaderTemplate = @"------------------------------------------------------------------------------
 <auto-generated>
     This code was generated by SpecFlow (http://www.specflow.org/).
     SpecFlow Version:{0}
     SpecFlow Generator Version:{1}
     Runtime Version:{2}

     Changes to this file may cause incorrect behavior and will be lost if
     the code is regenerated.
 </auto-generated>
------------------------------------------------------------------------------";

            var headerReader = new StringReader(string.Format(specFlowHeaderTemplate,
                GetCurrentSpecFlowVersion(),
                TestGeneratorFactory.GeneratorVersion,
                Environment.Version
                ));

            string line;
            while ((line = headerReader.ReadLine()) != null)
            {
                codeProvider.GenerateCodeFromStatement(new CodeCommentStatement(line), outputWriter, null);
            }

            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetStartRegionStatement("Designer generated code"), outputWriter, null);
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetDisableWarningsPragma(), outputWriter, null);
        }

        private void AddSpecFlowFooter(CodeDomProvider codeProvider, TextWriter outputWriter)
        {
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetEnableWarningsPragma(), outputWriter, null);
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetEndRegionStatement(), outputWriter, null);
        }

        private Version GetCurrentSpecFlowVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        #endregion
    }
}
