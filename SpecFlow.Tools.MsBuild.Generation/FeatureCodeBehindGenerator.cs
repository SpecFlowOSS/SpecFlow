using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FeatureCodeBehindGenerator : IDisposable
    {
        private CodeDomHelper _codeDomHelper;
        private ProjectSettings _projectSettings;
        private SpecFlowProject _specFlowProject;
        private ITestGenerator _testGenerator;

        public void InitializeProject(string projectPath, string rootNamespace, IEnumerable<string> generatorPlugins)
        {
            _specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(projectPath), rootNamespace);

            _projectSettings = _specFlowProject.ProjectSettings;

            _codeDomHelper = GetCodeDomHelper(_projectSettings);
            var testGeneratorFactory = new TestGeneratorFactory();

            _testGenerator = testGeneratorFactory.CreateGenerator(_projectSettings, generatorPlugins);
        }


        public GeneratedCodeBehindFile GenerateCodeBehindFile(string featureFile)
        {
            var featureFileInput = new FeatureFileInput(featureFile);
            var generatedFeatureFileName = Path.GetFileName(_testGenerator.GetTestFullPath(featureFileInput));

            var testGeneratorResult = _testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());

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