using System;
using System.CodeDom.Compiler;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;

namespace MonoDevelop.TechTalk.SpecFlow
{
	public class SingleFeatureFileGenerator : ISingleFileCustomTool
	{
		public IAsyncOperation Generate(IProgressMonitor monitor, ProjectFile featureFile, SingleFileCustomToolResult result)
		{
			return new ThreadAsyncOperation(() => {
				
                var ideSingleFileGenerator = new IdeSingleFileGenerator();

                ideSingleFileGenerator.GenerationError += 
                    delegate(TestGenerationError error)
                        {
                            result.Errors.Add(new CompilerError(featureFile.Name, error.Line + 1, error.LinePosition + 1, "0", error.Message));
                        };
                ideSingleFileGenerator.OtherError += 
                    delegate(Exception exception)
                        {
                            result.UnhandledException = exception;
                        };

                string outputFilePath = ideSingleFileGenerator.GenerateFile(featureFile.FilePath, null, () => new MonoDevelopGeneratorServices(featureFile.Project));
				result.GeneratedFilePath = outputFilePath;
				
			}, result);
		}
	}
}
