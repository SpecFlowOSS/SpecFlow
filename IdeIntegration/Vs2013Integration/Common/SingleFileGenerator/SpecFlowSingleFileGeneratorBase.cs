using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;

namespace TechTalk.SpecFlow.VsIntegration.SingleFileGenerator
{
    [ComVisible(true)]
    [Guid("7E4C9708-765C-46C8-8041-9ABD9927E0BC")]
    public abstract class SpecFlowSingleFileGeneratorBase : SingleFileGeneratorBase
    {
        protected SpecFlowSingleFileGeneratorBase() : base(".feature")
        {
        }

        private static string GetMessage(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;

            return ex.Message + " -> " + GetMessage(ex.InnerException);
        }

        protected override bool GenerateInternal(string inputFilePath, string inputFileContent, Project project, string defaultNamespace, Action<SingleFileGeneratorError> onError, out string generatedContent)
        {
            if (IsSharePointFeature(inputFileContent))
            {
                StringBuilder sharePointFeatureComment = new StringBuilder();
                sharePointFeatureComment.AppendFormat("/*SpecFlow tried to generate a test class file, but {0} appears to be a SharePoint feature.", Path.GetFileName(inputFilePath));
                sharePointFeatureComment.AppendLine("  The SpecFlow test class was not be generated in order to avoid errors in the SharePoint proejct*/");
                generatedContent = sharePointFeatureComment.ToString();
                return true;
            }

            var ideSingleFileGenerator = new IdeSingleFileGenerator();

            ideSingleFileGenerator.GenerationError +=
                delegate(TestGenerationError error)
                    {
                        onError(new SingleFileGeneratorError(error.Line, error.LinePosition, error.Message));
                    };
            ideSingleFileGenerator.OtherError +=
                delegate(Exception exception)
                    {
                        onError(new SingleFileGeneratorError(GetMessage(exception)));
                    };

            string outputContent = null;
            string outputFilePath = ideSingleFileGenerator.GenerateFile(inputFilePath, null, GeneratorServicesProvider(project), _ => inputFileContent, (_, oc) => { outputContent = oc; });

            generatedContent = outputContent;

            return outputFilePath != null;
        }

        protected abstract Func<GeneratorServices> GeneratorServicesProvider(Project project);

        protected override void AfterCodeGenerated(bool error)
        {
            base.AfterCodeGenerated(error);

            //TODO RefreshMsTestWindow(); //TODO
        }

        private bool IsSharePointFeature(string inputFileContent)
        {
            string trimmedInputFileContents = (inputFileContent ?? "").Trim();

            return trimmedInputFileContents.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>") &&
                   trimmedInputFileContents.Contains("<feature ") &&
                   trimmedInputFileContents.Contains("$SharePoint.Project.FileNameWithoutExtension$_$SharePoint.Feature.FileNameWithoutExtension$");
        }
    }
}