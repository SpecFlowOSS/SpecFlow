using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MonoDevelop.Projects;

using TechTalk.SpecFlow.Generator.Configuration;

namespace MonoDevelop.TechTalk.SpecFlow
{
	internal static class MonoDevelopProjectReader
	{
		public static SpecFlowProject CreateSpecFlowProjectFrom(Project project)
		{
			var specFlowProject = new SpecFlowProject();
			specFlowProject.ProjectFolder = project.BaseDirectory;
			specFlowProject.ProjectName = project.Name;
			
			string defaultNamespace = "Namespace";
			if (project is DotNetProject)
			{
				defaultNamespace = ((DotNetProject)project).GetDefaultNamespace(project.Name);
			}
			
			// No way to get AssemblyName right now, therefore we'll just use DefaultNamespace
			specFlowProject.AssemblyName = defaultNamespace;
			specFlowProject.DefaultNamespace = defaultNamespace;
			
			foreach (SolutionItemConfiguration configuration in project.Configurations)
			{
				MonoDevelop.Core.LoggingService.LogInfo(configuration.Name);
			}
			
			// TODO: Find out if we really need to add all the feature files everytime we generate
			foreach (ProjectFile projectFile in project.Files.Where(IsFeatureOrAppConfigFile))
			{
				string extension = Path.GetExtension(projectFile.Name);
				
				if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
				{
					string fileName = projectFile.FilePath.ToRelative(project.BaseDirectory);
					var featureFile = new SpecFlowFeatureFile(fileName);
					var customToolNamespace = projectFile.CustomToolNamespace;
					
					if (!String.IsNullOrEmpty(customToolNamespace))
						featureFile.CustomNamespace = customToolNamespace;
					
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
		
		private static bool IsFeatureOrAppConfigFile(ProjectFile projectFile)
        {
			string extension = Path.GetExtension(projectFile.Name);
            return extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase) 
				|| projectFile.Name.Equals("app.config", StringComparison.InvariantCultureIgnoreCase);
        }
	}
}
