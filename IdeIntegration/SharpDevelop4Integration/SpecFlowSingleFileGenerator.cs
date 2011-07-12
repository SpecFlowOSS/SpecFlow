using System;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
	/// <summary>
	/// Description of SpecFlowEngine.
	/// </summary>
	public sealed class SpecFlowSingleFileGenerator : ICustomTool
	{
		public bool IsFeatureFile(string fileName)
		{
			return String.Compare(Path.GetExtension(fileName), ".feature", true) == 0;
		}
		
		public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			context.RunAsync(
				()=> {
					string fileName = item.FileName;
					var projectNode = item.Project;
					SpecFlowProject specFlowProject = CreateSpecFlowProjectFrom(projectNode);
					var specFlowGenerator = new SpecFlowGenerator(specFlowProject);
					
					string outputFile = context.GetOutputFileName(item, ".feature");
					
					var specFlowFeatureFile = specFlowProject.GetOrCreateFeatureFile(fileName);
					
					var fileContents = File.ReadAllText(fileName);
					string outputFileContents;
					using(var reader = new StringReader(fileContents)) {
						using (var writer = new StringWriter(new StringBuilder())) {
							specFlowGenerator.GenerateTestFile(specFlowFeatureFile, projectNode.LanguageProperties.CodeDomProvider, reader, writer);
							outputFileContents = writer.ToString();
						}
					}
					File.WriteAllText(outputFile, outputFileContents);
					
					WorkbenchSingleton.SafeThreadCall(
						() => context.EnsureOutputFileIsInProject(item, outputFile));
				});
		}
		
		private SpecFlowProject CreateSpecFlowProjectFrom(IProject project)
		{
			var specFlowProject = new SpecFlowProject();
			specFlowProject.ProjectSettings.ProjectFolder = project.Directory;
            specFlowProject.ProjectSettings.ProjectName = project.Name;
            specFlowProject.ProjectSettings.AssemblyName = project.AssemblyName;
            specFlowProject.ProjectSettings.DefaultNamespace = project.RootNamespace;

            var generatorConfig = specFlowProject.Configuration.GeneratorConfiguration;
			
			foreach (var projectFile in project.Items.OfType<FileProjectItem>().Where(IsFeatureOrAppConfigFile))
			{
				string extension = Path.GetExtension(projectFile.FileName);
				if (extension != null && extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
				{
					string fileName = FileUtilities.GetRelativePath(projectFile.FileName, project.Directory);
                    var featureFile = new FeatureFileInput(fileName);
					
					specFlowProject.FeatureFiles.Add(featureFile);
				}

                if (extension != null && extension.Equals(".config", StringComparison.InvariantCultureIgnoreCase))
				{
					string configContent = File.ReadAllText(projectFile.FileName);
                    GeneratorConfigurationReader.UpdateConfigFromFileContent(generatorConfig, configContent);
				}
			}
			
			return specFlowProject;
		}
		
		private static bool IsFeatureOrAppConfigFile(FileProjectItem projectFile)
		{
			string fileName = Path.GetFileName(projectFile.FileName);
			string extension = Path.GetExtension(fileName);
            if (extension == null || fileName == null)
                return false;

            return extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase)
				|| fileName.Equals("app.config", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
