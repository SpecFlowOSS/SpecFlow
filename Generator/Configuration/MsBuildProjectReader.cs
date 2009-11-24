using System;
using System.IO;
using System.Linq;
using Microsoft.Build.BuildEngine;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public static class MsBuildProjectReader
    {
        public static SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFile)
        {
            projectFile = Path.GetFullPath(projectFile);
            
            Project project = Engine.GlobalEngine.GetLoadedProject(projectFile);
            if (project == null)
            {
                project = new Project();
                project.Load(projectFile, ProjectLoadSettings.IgnoreMissingImports);
            }

            string projectFolder = Path.GetDirectoryName(projectFile);

            SpecFlowProject specFlowProject = new SpecFlowProject();
            specFlowProject.ProjectFolder = projectFolder;
            specFlowProject.ProjectName = Path.GetFileNameWithoutExtension(projectFile);
            specFlowProject.AssemblyName = project.GetEvaluatedProperty("AssemblyName");
            specFlowProject.DefaultNamespace = project.GetEvaluatedProperty("RootNamespace");

            var items = project.GetEvaluatedItemsByName("None").Cast<BuildItem>()
                .Concat(project.GetEvaluatedItemsByName("Content").Cast<BuildItem>());
            foreach (BuildItem item in items)
            {
                var extension = Path.GetExtension(item.FinalItemSpec);
                if (extension.Equals(".feature", StringComparison.InvariantCultureIgnoreCase))
                {
                    var featureFile = new SpecFlowFeatureFile(item.FinalItemSpec);
                    var ns = item.GetEvaluatedMetadata("CustomToolNamespace");
                    if (!String.IsNullOrEmpty(ns))
                        featureFile.CustomNamespace = ns;
                    specFlowProject.FeatureFiles.Add(featureFile);
                }

                if (Path.GetFileName(item.FinalItemSpec).Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
                {
                    GeneratorConfigurationReader.UpdateConfigFromFile(specFlowProject.GeneratorConfiguration, Path.Combine(projectFolder, item.FinalItemSpec));
                }
            }
            return specFlowProject;
        }
    }
}