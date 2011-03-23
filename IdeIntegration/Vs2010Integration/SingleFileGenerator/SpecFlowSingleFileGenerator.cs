using System;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using TechTalk.SpecFlow.VsIntegration.Common;
using VSLangProj80;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    [ComVisible(true)]
    [Guid("44F8C2E2-18A9-4B97-B830-6BCD0CAA161C")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "C# SpecFlow Generator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "VB.NET SpecFlow Generator", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [CodeGeneratorRegistration(typeof(SpecFlowSingleFileGenerator), "Silverlight SpecFlow Generator", GuidList.vsContextGuidSilverlightProject, GeneratesDesignTimeSource = true, FileExtension = ".feature")]
    [ProvideObject(typeof(SpecFlowSingleFileGenerator))]
    public class SpecFlowSingleFileGenerator : SpecFlowSingleFileGeneratorBase
    {
        protected override void RefreshMsTestWindow()
        {
            //the automatic refresh of the test window causes problems in VS2010 -> skip
        }

        private ITestGeneratorFactory GetTestGeneratorFactory()
        {
            //TODO: create it in new AppDomain if neccessary
            return new TestGeneratorFactory();
        }

        protected override string GenerateCode(string inputFileContent)
        {
            ITestGeneratorFactory testGeneratorFactory = GetTestGeneratorFactory();

            var configHolder = DteProjectReader.LoadConfigHolderFromProject(CurrentProject);
            var testGenerator = testGeneratorFactory.CreateGenerator(configHolder);

            string projectFolder = VsxHelper.GetProjectFolder(CurrentProject);
            var fullPath = Path.GetFullPath(Path.Combine(projectFolder, CodeFilePath));
            FeatureFileInput featureFileInput = 
                new FeatureFileInput(fullPath,
                    FileSystemHelper.GetRelativePath(fullPath, projectFolder),
                    null, //TODO
                    new StringReader(inputFileContent));

            StringWriter outputWriter = new StringWriter();
            testGenerator.GenerateTestFile(featureFileInput, outputWriter, GetGeneratorSettings(CurrentProject));
            return outputWriter.ToString();
        }

        private GenerationSettings GetGeneratorSettings(Project project)
        {
            var tergetLanguage = VsProjectScope.GetTargetLanguage(CurrentProject);
            switch (tergetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    return new GenerationSettings(
                        Generator.GenerationTargetLanguage.CSharp, "3.0",
                        GenerationTargetPlatform.MsNet, "3.5",
                        VsxHelper.GetProjectDefaultNamespace(project));
                case GenerationTargetLanguage.VB:
                    return new GenerationSettings(
                        Generator.GenerationTargetLanguage.VB, "7.0",
                        GenerationTargetPlatform.MsNet, "3.5",
                        VsxHelper.GetProjectDefaultNamespace(project));
                default:
                    throw new NotSupportedException("target language not supported");
            }
        }
    }
}
