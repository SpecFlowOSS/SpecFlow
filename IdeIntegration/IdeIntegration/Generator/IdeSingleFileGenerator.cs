using System;
using System.Linq;
using System.IO;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class IdeSingleFileGenerator
    {
        public event Action<TestGenerationError> GenerationError;
        public event Action<Exception> OtherError;

        public string GenerateFile(string inputFilePath, string outputFilePath, Func<GeneratorServices> generatorServicesProvider, Func<string, string> inputFileContentProvider = null, Action<string, string> outputFileContentWriter = null)
        {
            outputFileContentWriter = outputFileContentWriter ?? File.WriteAllText;
            inputFileContentProvider = inputFileContentProvider ?? File.ReadAllText;

            GeneratorServices generatorServices;
            ProjectSettings projectSettings;
            CodeDomHelper codeDomHelper;

            try
            {
                generatorServices = generatorServicesProvider();
                projectSettings = generatorServices.GetProjectSettings();
                codeDomHelper = GenerationTargetLanguage.CreateCodeDomHelper(projectSettings.ProjectPlatformSettings.Language);
            }
            catch(Exception ex)
            {
                OnOtherError(ex);
                return null;
            }

            string inputFileContent;
            try
            {
                inputFileContent = inputFileContentProvider(inputFilePath);
            }
            catch(Exception ex)
            {
                OnOtherError(ex);
                return null;
            }

            string outputFileContent = Generate(inputFilePath, inputFileContent, generatorServices, codeDomHelper, projectSettings);

            try
            {
                if (outputFilePath == null)
                    outputFilePath = inputFilePath + GenerationTargetLanguage.GetExtension(projectSettings.ProjectPlatformSettings.Language);

                outputFileContentWriter(outputFilePath, outputFileContent);

                return outputFilePath;
            }
            catch(Exception ex)
            {
                OnOtherError(ex);
                return null;
            }
        }

        private string Generate(string inputFilePath, string inputFileContent, GeneratorServices generatorServices, CodeDomHelper codeDomHelper, ProjectSettings projectSettings)
        {
            string outputFileContent;
            try
            {
                TestGeneratorResult generationResult = GenerateCode(inputFilePath, inputFileContent, generatorServices, projectSettings);

                if (generationResult.Success)
                    outputFileContent = generationResult.GeneratedTestCode;
                else
                    outputFileContent = GenerateError(generationResult, codeDomHelper);
            }
            catch (Exception ex)
            {
                outputFileContent = GenerateError(ex, codeDomHelper);
            }
            return outputFileContent;
        }

        private TestGeneratorResult GenerateCode(string inputFilePath, string inputFileContent, GeneratorServices generatorServices, ProjectSettings projectSettings)
        {
            using (var testGenerator = generatorServices.CreateTestGenerator())
            {
                var fullPath = Path.GetFullPath(Path.Combine(projectSettings.ProjectFolder, inputFilePath));
                FeatureFileInput featureFileInput =
                    new FeatureFileInput(FileSystemHelper.GetRelativePath(fullPath, projectSettings.ProjectFolder))
                        {
                            FeatureFileContent = inputFileContent
                        };
                return testGenerator.GenerateTestFile(featureFileInput, new GenerationSettings());
            }
        }

        private string GenerateError(TestGeneratorResult generationResult, CodeDomHelper codeDomHelper)
        {
            var errorsArray = generationResult.Errors.ToArray();

            foreach (var testGenerationError in errorsArray)
            {
                OnGenerationError(testGenerationError);
            }

            return string.Join(Environment.NewLine, errorsArray.Select(e => codeDomHelper.GetErrorStatementString(e.Message)).ToArray());
        }

        private string GenerateError(Exception ex, CodeDomHelper codeDomHelper)
        {
            TestGenerationError testGenerationError = new TestGenerationError(ex);
            OnGenerationError(testGenerationError);
            return codeDomHelper.GetErrorStatementString(testGenerationError.Message);
        }

        protected virtual void OnGenerationError(TestGenerationError testGenerationError)
        {
            if (GenerationError != null)
                GenerationError(testGenerationError);
        }

        protected virtual void OnOtherError(Exception exception)
        {
            if (OtherError != null)
                OtherError(exception);
        }
    }
}
