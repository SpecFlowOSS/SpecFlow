
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
	/// <summary>
	/// Description of SpecFlowEngine.
	/// </summary>
	public sealed class SpecFlowEngine
	{
		private static SpecFlowEngine instance = new SpecFlowEngine();
		
		public static SpecFlowEngine Instance {
			get {
				return instance;
			}
		}
		
		private SpecFlowEngine()
		{
		}
		
		/// <summary>
		/// Determines if the specified project is a SpecFlow project by checking the project references.
		/// </summary>
		public bool IsSpecFlowProject(IProject project)
		{
			var specFlowAssembly = typeof(ITestRunner).Assembly;
			return project.GetItemsOfType(typeof(ReferenceProjectItem)).Cast<ReferenceProjectItem>()
				.Any(x => x.AssemblyName == specFlowAssembly.FullName);
		}
		
		public void ProcessFile(string fileName)
		{
			if(IsFeatureFile(fileName)) {
				GenerateCodeBehind(fileName);
			}
		}
		
		public bool IsFeatureFile(string fileName)
		{
			return String.Compare(Path.GetExtension(fileName), ".feature");
		}
		
		private void GenerateCode(string fileName)
		{
			var solution = ProjectService.OpenSolution;
			if(solution == null) {
				throw new CodeGenerationException(String.Format(
					"Failed to generate test code for '{0}' as there is no open solution.", fileName));
			}			
			
				var projectNode = solution.FindProjectContainingFile(fileName);		
				if(projectNode == null) {
					throw new CodeGenerationException(String.Format(
					"Failed to generate test code for '{0}' since feature file didn't belong to a project.", fileName));
				}
				
				SpecFlowProject specFlowProject = CreateSpecFlowProjectFrom(projectNode);
				var specFlowGenerator = new SpecFlowGenerator(specFlowProject);
				
				projectNode
				string testCodeFileName = Path.ChangeExtension(fileName, ".feature.cs");
				using (var writer = new StringWriter(new StringBuilder()))
				using (var reader = new StringReader(File.ReadAllText(fileName)))
				{
					SpecFlowFeatureFile specFlowFeatureFile = specFlowProject.GetOrCreateFeatureFile(fileName);
					specFlowGenerator.GenerateTestFile(specFlowFeatureFile, codeProvider, reader, writer);
					File.WriteAllText(outputFile, writer.ToString());
				}
		}
		
		private string TestCodeFileExtention(IProject project)
		{
			if(project.Language == "C#") {
				return ".feature.cs";
			} else if(project.Language == "VBNET") {
				return ".feature.vb";
			} else {
				return ".feature.cs";
			}
		}
		
		private SpecFlowProject CreateSpecFlowProjectFrom(IProject project)
		{
			var specFlowProject = new SpecFlowProject();
			specFlowProject.ProjectFolder = project.Directory;
			specFlowProject.ProjectName = project.Name;
			specFlowProject.GeneratorConfiguration.ToolLanguage
			// No way to get AssemblyName right now, therefore we'll just use DefaultNamespace
			specFlowProject.AssemblyName = project.AssemblyName;
			specFlowProject.DefaultNamespace = project.RootNamespace;
			
			// TODO: Find out if we really need to add all the feature files everytime we generate
			foreach (var projectFile in project.GetItemsOfType(typeof(FileProjectItem)).Cast<FileProjectItem>().Where(IsFeatureOrAppConfigFile))
			{
				string extension = Path.GetExtension(projectFile.FileName);				
				if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
				{
					string fileName = FileUtilities.GetRelativePath(projectFile.FileName, project.BaseDirectory);
					var featureFile = new SpecFlowFeatureFile(fileName);
					
					specFlowProject.FeatureFiles.Add(featureFile);
				}
				
				if (extension.Equals(".config", StringComparison.InvariantCultureIgnoreCase))
				{
					string configContent = File.ReadAllText(projectFile.FilePath);
					GeneratorConfigurationReader.UpdateConfigFromFileContent(specFlowProject.GeneratorConfiguration, configContent);
				}
			}
			
			return specFlowProject;
		}
		
		private static bool IsFeatureOrAppConfigFile(FileProjectItem projectFile)
        {
			string fileName = projectFile.FileName;
			string extension = Path.GetExtension(fileName);
            return extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase) 
				|| projectFile.Name.Equals("app.config", StringComparison.InvariantCultureIgnoreCase);
        }
	}
}
