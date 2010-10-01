
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
	/// <summary>
	/// Description of SpecFlowEngine.
	/// </summary>
	public sealed class SpecFlowSingleFileGenerator : ICustomTool
	{
		public SpecFlowSingleFileGenerator()
		{
		}
		
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
					
					SpecFlowFeatureFile specFlowFeatureFile = specFlowProject.GetOrCreateFeatureFile(fileName);
					
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
						() => {
							context.EnsureOutputFileIsInProject(item, outputFile);
						});
				});
		}
		
//		private void GenerateCodeAsync(FileProjectItem item)
//		{
//			string fileName = item.FileName;
//			if(IsFeatureFile(fileName)) {
//				var projectNode = item.Project;
//				var codeProvider = projectNode.LanguageProperties.CodeDomProvider;
//
//				SpecFlowProject specFlowProject = CreateSpecFlowProjectFrom(projectNode);
//				var specFlowGenerator = new SpecFlowGenerator(specFlowProject);
//
//				string outputFile =
//
//					SpecFlowFeatureFile specFlowFeatureFile = specFlowProject.GetOrCreateFeatureFile(fileName);
//
//				try {
//					var fileContents = File.ReadAllText(fileName);
//					string outputFileContents;
//					using(var reader = new StringReader(fileContents)) {
//						using (var writer = new StringWriter(new StringBuilder())) {
//							specFlowGenerator.GenerateTestFile(specFlowFeatureFile, codeProvider, reader, writer);
//							outputFileContents = writer.ToString();
//						}
//					}
//					File.WriteAllText(outputFile, outputFileContents);
//					WorkbenchSingleton.SafeThreadAsyncCall(
//						() => {
//							FileUtility.RaiseFileSaved(new FileNameEventArgs(outputFile));
//							if(!projectNode.IsFileInProject(outputFile)) {
//								var outputFileProjectItem = new FileProjectItem(projectNode, ItemType.Compile);
//								outputFileProjectItem.
//									outputFileProjectItem.FileName = outputFile;
//								ProjectService.AddProjectItem(projectNode, outputFileProjectItem);
//								ProjectBrowserPad.RefreshViewAsync();
//								projectNode.Save();
//							}
//						});
//				} catch(IOException) {
//					// IOExceptions are possible and expected since the read/writer is asynchrously accessing shared files which could be locked (for instance due to a save).
//				}
//			}
//		}
//
		private SpecFlowProject CreateSpecFlowProjectFrom(IProject project)
		{
			var specFlowProject = new SpecFlowProject();
			specFlowProject.ProjectFolder = project.Directory;
			specFlowProject.ProjectName = project.Name;
			specFlowProject.AssemblyName = project.AssemblyName;
			specFlowProject.DefaultNamespace = project.RootNamespace;
			
			var generatorConfig = specFlowProject.GeneratorConfiguration;
			
			foreach (var projectFile in project.Items.OfType<FileProjectItem>().Where(IsFeatureOrAppConfigFile))
			{
				string extension = Path.GetExtension(projectFile.FileName);
				if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
				{
					string fileName = FileUtilities.GetRelativePath(projectFile.FileName, project.Directory);
					var featureFile = new SpecFlowFeatureFile(fileName);
					
					specFlowProject.FeatureFiles.Add(featureFile);
				}
				
				if (extension.Equals(".config", StringComparison.InvariantCultureIgnoreCase))
				{
					string configContent = File.ReadAllText(projectFile.FileName);
					GeneratorConfigurationReader.UpdateConfigFromFileContent(specFlowProject.GeneratorConfiguration, configContent);
				}
			}
			
			return specFlowProject;
		}
		
		private static bool IsFeatureOrAppConfigFile(FileProjectItem projectFile)
		{
			string fileName = Path.GetFileName(projectFile.FileName);
			string extension = Path.GetExtension(fileName);
			return extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase)
				|| fileName.Equals("app.config", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
