using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    public class TestGenerator : ErrorHandlingTestGenerator
    {
        protected readonly GeneratorConfiguration generatorConfiguration;
        protected readonly ProjectSettings projectSettings;
        protected readonly ITestHeaderWriter testHeaderWriter;

        public TestGenerator(GeneratorConfiguration generatorConfiguration, ProjectSettings projectSettings, ITestHeaderWriter testHeaderWriter)
        {
            if (generatorConfiguration == null) throw new ArgumentNullException("generatorConfiguration");
            if (projectSettings == null) throw new ArgumentNullException("projectSettings");
            if (testHeaderWriter == null) throw new ArgumentNullException("testHeaderWriter");

            this.generatorConfiguration = generatorConfiguration;
            this.testHeaderWriter = testHeaderWriter;
            this.projectSettings = projectSettings;
        }

        protected virtual CodeDomProvider CreateCodeDomProvider(ProjectPlatformSettings settings)
        {
            switch (settings.Language)
            {
                case GenerationTargetLanguage.CSharp:
                    return new CSharpCodeProvider();
                case GenerationTargetLanguage.VB:
                    return new VBCodeProvider();
                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual CodeDomHelper CreateCodeDomHelper(CodeDomProvider codeDomProvider, GenerationSettings settings)
        {
            return new CodeDomHelper(codeDomProvider);
        }

        protected override TestGeneratorResult GenerateTestFileWithExceptions(FeatureFileInput featureFileInput, GenerationSettings settings)
        {
            if (featureFileInput == null) throw new ArgumentNullException("featureFileInput");
            if (settings == null) throw new ArgumentNullException("settings");

            StringWriter outputStringWriter = new StringWriter();
            var outputWriter = new IndentProcessingWriter(outputStringWriter);

            var codeProvider = CreateCodeDomProvider(projectSettings.ProjectPlatformSettings);
            CodeDomHelper codeDomHelper = CreateCodeDomHelper(codeProvider, settings);

            var codeNamespace = GenerateTestFileCode(featureFileInput, codeDomHelper);
            var options = new CodeGeneratorOptions
            {
                BracingStyle = "C"
            };

            AddSpecFlowHeader(codeProvider, outputWriter, codeDomHelper);
            codeProvider.GenerateCodeFromNamespace(codeNamespace, outputWriter, options);
            AddSpecFlowFooter(codeProvider, outputWriter, codeDomHelper);
            outputWriter.Flush();

            return new TestGeneratorResult(outputStringWriter.ToString(), false);
        }

        private CodeNamespace GenerateTestFileCode(FeatureFileInput featureFileInput, CodeDomHelper codeDomHelper)
        {
            string targetNamespace = GetTargetNamespace(featureFileInput) ?? "SpecFlow.GeneratedTests";

            SpecFlowLangParser parser = new SpecFlowLangParser(generatorConfiguration.FeatureLanguage);
            Feature feature;
            using (var contentReader = featureFileInput.GetFeatureFileContentReader(projectSettings))
            {
                feature = parser.Parse(contentReader, featureFileInput.GetFullPath(projectSettings));
            }

            IUnitTestGeneratorProvider generatorProvider = ConfigurationServices.CreateInstance<IUnitTestGeneratorProvider>(generatorConfiguration.GeneratorUnitTestProviderType);
            codeDomHelper.InjectIfRequired(generatorProvider);

            ISpecFlowUnitTestConverter testConverter = new SpecFlowUnitTestConverter(generatorProvider, codeDomHelper, generatorConfiguration.AllowDebugGeneratedFiles, generatorConfiguration.AllowRowTests);

            var codeNamespace = testConverter.GenerateUnitTestFixture(feature, null, targetNamespace);
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



        #region Header & Footer
        protected override Version DetectGeneratedTestVersionWithExceptions(FeatureFileInput featureFileInput)
        {
            var generatedTestFileContent = featureFileInput.GeneratedTestFileContent;
            if (generatedTestFileContent == null)
            {
                var generatedTestPath = featureFileInput.GetGeneratedTestFullPath(projectSettings);
                if (generatedTestPath == null)
                    return null;
                generatedTestFileContent = File.ReadAllText(generatedTestPath);
            }

            return testHeaderWriter.DetectGeneratedTestVersion(generatedTestFileContent);
        }

        private void AddSpecFlowHeader(CodeDomProvider codeProvider, TextWriter outputWriter, CodeDomHelper codeDomHelper)
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
        }

        private void AddSpecFlowFooter(CodeDomProvider codeProvider, TextWriter outputWriter, CodeDomHelper codeDomHelper)
        {
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetEndRegionStatement(), outputWriter, null);
        }

        private Version GetCurrentSpecFlowVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        #endregion
    }
}
