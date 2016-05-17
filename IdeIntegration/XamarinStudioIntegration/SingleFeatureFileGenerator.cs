using System;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;

namespace MonoDevelop.TechTalk.SpecFlow
{
	public class SingleFeatureFileGenerator : ISingleFileCustomTool
	{
		public async Task Generate(ProgressMonitor monitor, ProjectFile featureFile, SingleFileCustomToolResult result)
		{
			await Task.Run(() => 
            {				
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
				
			});
		}
	}
}
