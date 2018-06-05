using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FeatureCodeBehindGenerator : IDisposable
    {
        private readonly TaskLoggingHelper _logger;
        private CodeDomHelper _codeDomHelper;
        private ProjectSettings _projectSettings;
        private SpecFlowProject _specFlowProject;

        public FeatureCodeBehindGenerator(TaskLoggingHelper logger)
        {
            _logger = logger;
        }

        public void InitializeProject(string projectPath, string rootNamespace)
        {
            _logger.LogWithNameTag(_logger.LogMessage, nameof(InitializeProject)+$"({projectPath})");

            _specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(projectPath), rootNamespace);

            _projectSettings = _specFlowProject.ProjectSettings;

            _codeDomHelper = GetCodeDomHelper(_projectSettings);
        }


        public GeneratedCodeBehindFile GenerateCodeBehindFile(string featureFile)
        {
            _logger.LogWithNameTag(_logger.LogMessage, nameof(GenerateCodeBehindFile)+$"{featureFile}");

            var testGeneratorFactory = new TestGeneratorFactory();

            var testGenerator = testGeneratorFactory.CreateGenerator(_projectSettings);


            FeatureFileInput featureFileInput = new FeatureFileInput(featureFile);
            var generatedFeatureFileName = Path.GetFileName(testGenerator.GetTestFullPath(featureFileInput));


            var testGeneratorResult = testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());

            string outputFileContent;
            if (testGeneratorResult.Success)
            {
                outputFileContent = testGeneratorResult.GeneratedTestCode;
            }
            else
            {
                outputFileContent = GenerateError(testGeneratorResult, _codeDomHelper);
            }

            return new GeneratedCodeBehindFile()
            {
                Filename = generatedFeatureFileName,
                Content = outputFileContent
            };
        }

        private string GenerateError(TestGeneratorResult generationResult, CodeDomHelper codeDomHelper)
        {
            var errorsArray = generationResult.Errors.ToArray();


            return string.Join(Environment.NewLine, errorsArray.Select(e => codeDomHelper.GetErrorStatementString(e.Message)).ToArray());
        }

        private CodeDomHelper GetCodeDomHelper(ProjectSettings projectSettings)
        {
            return GenerationTargetLanguage.CreateCodeDomHelper(projectSettings.ProjectPlatformSettings.Language);
        }

        public void Dispose()
        {
            
        }
    }
}