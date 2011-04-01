using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MonoDevelop.Projects;

using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace MonoDevelop.TechTalk.SpecFlow
{
	internal static class MonoDevelopProjectReader
	{
		public static SpecFlowProject CreateSpecFlowProjectFrom(Project project)
		{
			var specFlowProject = new SpecFlowProject();
			specFlowProject.ProjectSettings.ProjectFolder = project.BaseDirectory;
            specFlowProject.ProjectSettings.ProjectName = project.Name;
			
			string defaultNamespace = "Namespace";
			if (project is DotNetProject)
			{
				defaultNamespace = ((DotNetProject)project).GetDefaultNamespace(project.Name);
			}
			
			// No way to get AssemblyName right now, therefore we'll just use DefaultNamespace
            specFlowProject.ProjectSettings.AssemblyName = defaultNamespace;
			specFlowProject.ProjectSettings.DefaultNamespace = defaultNamespace;
			
			// TODO: Find out if we really need to add all the feature files everytime we generate
			foreach (ProjectFile projectFile in project.Files.Where(IsFeatureOrAppConfigFile))
			{
				string extension = Path.GetExtension(projectFile.Name);
				
				if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
				{
					string fileName = projectFile.FilePath.ToRelative(project.BaseDirectory);
                    var featureFile = new FeatureFileInput(fileName);
					var customToolNamespace = projectFile.CustomToolNamespace;
					
					if (!String.IsNullOrEmpty(customToolNamespace))
						featureFile.CustomNamespace = customToolNamespace;
					
					specFlowProject.FeatureFiles.Add(featureFile);
				}
				
				if (extension.Equals(".config", StringComparison.InvariantCultureIgnoreCase))
				{
					string configContent = File.ReadAllText(projectFile.FilePath);
                    GeneratorConfigurationReader.UpdateConfigFromFileContent(specFlowProject.Configuration.GeneratorConfiguration, configContent);
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
