using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class ProjectReader : ISpecFlowProjectReader
    {
        private readonly IGeneratorConfigurationProvider _configurationLoader;
        private readonly ProjectLanguageReader _languageReader;

        public ProjectReader(IGeneratorConfigurationProvider configurationLoader, ProjectLanguageReader languageReader)
        {
            _configurationLoader = configurationLoader;
            _languageReader = languageReader;
        }

        public SpecFlowProject ReadSpecFlowProject(string projectFilePath, string rootNamespace)
        {
            try
            {
                var projectFolder = Path.GetDirectoryName(projectFilePath);

                var specFlowProject = new SpecFlowProject();
                specFlowProject.ProjectSettings.ProjectFolder = projectFolder;
                specFlowProject.ProjectSettings.ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
                specFlowProject.ProjectSettings.DefaultNamespace = rootNamespace;
                specFlowProject.ProjectSettings.ProjectPlatformSettings.Language = _languageReader.GetLanguage(projectFilePath);

      
                specFlowProject.ProjectSettings.ConfigurationHolder = GetSpecFlowConfigurationHolder(projectFolder);

                if (specFlowProject.ProjectSettings.ConfigurationHolder != null)
                {
                    specFlowProject.Configuration = _configurationLoader.LoadConfiguration(specFlowProject.ProjectSettings.ConfigurationHolder);
                }

                return specFlowProject;
            }
            catch (Exception e)
            {
                throw new Exception("Error when reading project file.", e);
            }
        }

        private SpecFlowConfigurationHolder GetSpecFlowConfigurationHolder(string projectFolder)
        {
            string jsonConfigPath = Path.Combine(projectFolder, "specflow.json");
            if (File.Exists(jsonConfigPath))
            {
                var configFileContent = File.ReadAllText(jsonConfigPath);
                return new SpecFlowConfigurationHolder(ConfigSource.Json, configFileContent);
            }

            string appConfigPath = Path.Combine(projectFolder, "app.config");
            if (File.Exists(appConfigPath))
            {
                var configFilePath = Path.Combine(projectFolder, appConfigPath);
                var configFileContent = File.ReadAllText(configFilePath);
                return GetConfigurationHolderFromFileContent(configFileContent);
            }

            return new SpecFlowConfigurationHolder();
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