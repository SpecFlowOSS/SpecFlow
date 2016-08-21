using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Build.Evaluation;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class MsBuildProjectReader : ISpecFlowProjectReader
    {
        private readonly IGeneratorConfigurationProvider configurationLoader;

        public MsBuildProjectReader(IGeneratorConfigurationProvider configurationLoader)
        {
            this.configurationLoader = configurationLoader;
        }

        public static SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath, ITraceListener traceListener)
        {
            return new MsBuildProjectReader(new GeneratorConfigurationProvider(new ConfigurationLoader())).ReadSpecFlowProject(projectFilePath);
        }

        public SpecFlowProject ReadSpecFlowProject(string projectFilePath)
        {
            Microsoft.Build.Evaluation.Project project = ProjectCollection.GlobalProjectCollection.LoadProject(projectFilePath);

            string projectFolder = Path.GetDirectoryName(projectFilePath);

            var specFlowProject = new SpecFlowProject();
            specFlowProject.ProjectSettings.ProjectFolder = projectFolder;
            specFlowProject.ProjectSettings.ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
            specFlowProject.ProjectSettings.AssemblyName = project.AllEvaluatedProperties.First(x=>x.Name=="AssemblyName").EvaluatedValue;
            specFlowProject.ProjectSettings.DefaultNamespace =project.AllEvaluatedProperties.First(x=>x.Name=="RootNamespace").EvaluatedValue;



            specFlowProject.ProjectSettings.ProjectPlatformSettings.Language = GetLanguage(project);

            foreach (ProjectItem item in project.FeatureFiles())
            {
                var featureFile = new FeatureFileInput(item.EvaluatedInclude);
                var ns = item.GetMetadataValue("CustomToolNamespace");
                if (!String.IsNullOrEmpty(ns))
                    featureFile.CustomNamespace = ns;
                specFlowProject.FeatureFiles.Add(featureFile);
                               
            }

            SpecFlowConfigurationHolder configurationHolder = null;

            ProjectItem appConfigItem = project.ApplicationConfigurationFile();
            if (appConfigItem != null)
            {
                var configFilePath = Path.Combine(projectFolder, appConfigItem.EvaluatedInclude);
                var configFileContent = File.ReadAllText(configFilePath);
                configurationHolder = GetConfigurationHolderFromFileContent(configFileContent);
            }

            ProjectItem specflowJsonItem = project.SpecFlowJsonConfig();
            if (specflowJsonItem != null)
            {
                var configFilePath = Path.Combine(projectFolder, specflowJsonItem.EvaluatedInclude);
                var configFileContent = File.ReadAllText(configFilePath);
                configurationHolder = new SpecFlowConfigurationHolder(ConfigSource.Json, configFileContent);
            }

            if (configurationHolder != null)
            {
                specFlowProject.ProjectSettings.ConfigurationHolder = configurationHolder;
                specFlowProject.Configuration = configurationLoader.LoadConfiguration(configurationHolder);
            }

            return specFlowProject;
        }

        private string GetLanguage(Microsoft.Build.Evaluation.Project project)
        {
            if (project.Imports.Any(i => i.ImportingElement.Project.Contains("Microsoft.VisualBasic.targets")))
                return GenerationTargetLanguage.VB;

            if (project.Imports.Any(i => i.ImportingElement.Project.Contains("Microsoft.CSharp.targets")))
                return GenerationTargetLanguage.CSharp;

            return GenerationTargetLanguage.CSharp;
        }

        private static SpecFlowConfigurationHolder GetConfigurationHolderFromFileContent(string configFileContent)
        {
            try
            {
                var configDocument = new XmlDocument();
                configDocument.LoadXml(configFileContent);

                return new SpecFlowConfigurationHolder(configDocument.SelectSingleNode("/configuration/specFlow"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Config load error");
                return new SpecFlowConfigurationHolder();
            }
        }
    }
}