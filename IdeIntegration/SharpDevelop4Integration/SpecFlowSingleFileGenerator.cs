using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using TechTalk.SpecFlow.IdeIntegration.Generator;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
    /// <summary>
	/// Description of SpecFlowEngine.
	/// </summary>
	public class SpecFlowSingleFileGenerator : ICustomTool
	{
        public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
            context.RunAsync(() =>
            {
                var ideSingleFileGenerator = new IdeSingleFileGenerator();

                string outputFilePath = context.GetOutputFileName(item, ".feature");
                ideSingleFileGenerator.GenerateFile(item.FileName, outputFilePath, () => new SharpDevelop4GeneratorServices(item.Project));

                WorkbenchSingleton.SafeThreadCall(
                    () => context.EnsureOutputFileIsInProject(item, outputFilePath));
            });
		}
	}
}
