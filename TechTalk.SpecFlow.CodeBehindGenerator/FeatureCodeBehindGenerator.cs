using System;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Rpc.Server;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class FeatureCodeBehindGenerator : IFeatureCodeBehindGenerator
    {
        private readonly BuildServerController _buildServerController;
        private CodeDomHelper _codeDomHelper;
        private string _projectPath;
        private ProjectSettings _projectSettings;
        private SpecFlowProject _specFlowProject;

        public FeatureCodeBehindGenerator(BuildServerController buildServerController)
        {
            _buildServerController = buildServerController;
        }

        public void InitializeProject(string projectPath)
        {
            _projectPath = projectPath;

            _specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(projectPath));

            _projectSettings = _specFlowProject.ProjectSettings;

            _codeDomHelper = GetCodeDomHelper(_projectSettings);
        }


        public GeneratedCodeBehindFile GenerateCodeBehindFile(string featureFile)
        {
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

        public void Ping()
        {
        }

        public void Shutdown()
        {
            _buildServerController.Stop();
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
    }
}