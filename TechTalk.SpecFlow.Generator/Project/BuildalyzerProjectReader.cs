using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Buildalyzer;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class BuildalyzerProjectReader : ISpecFlowProjectReader
    {
        private readonly IGeneratorConfigurationProvider _configurationLoader;
        private readonly BuildalyzerLanguageReader _languageReader;

        public BuildalyzerProjectReader(IGeneratorConfigurationProvider configurationLoader, BuildalyzerLanguageReader languageReader)
        {
            _configurationLoader = configurationLoader;
            _languageReader = languageReader;
        }

        public SpecFlowProject ReadSpecFlowProject(string projectFilePath)
        {
            //if (projectFilePath.EndsWith("DefaultTestProject.csproj"))
            //{
            //    Debugger.Launch();
            //}

            var logWriter = new StringWriter();
            logWriter.WriteLine("Using BuildAlyzer");

            string binLog = Path.Combine(Path.GetDirectoryName(projectFilePath), "buildalayzer.binlog");

            string debugInfo = String.Empty;

            try
            {
                var manager = new AnalyzerManager(new AnalyzerManagerOptions()
                {
                    LogWriter = logWriter,
                    LoggerVerbosity = LoggerVerbosity.Detailed,
                });

                
                var analyzer = manager.GetProject(projectFilePath).WithBinaryLog(binLog);
                debugInfo += $"BinLog: {binLog}" + Environment.NewLine;

                debugInfo += String.Join(Environment.NewLine, analyzer.GlobalProperties.Select(kv => $"{kv.Key}: {kv.Value}"));

                var project = analyzer.Project;

                

                var projectFolder = Path.GetDirectoryName(projectFilePath);

                var specFlowProject = new SpecFlowProject();
                specFlowProject.ProjectSettings.ProjectFolder = projectFolder;
                specFlowProject.ProjectSettings.ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
                specFlowProject.ProjectSettings.AssemblyName = project.Properties.First(x => x.Name == "AssemblyName").EvaluatedValue;
                specFlowProject.ProjectSettings.DefaultNamespace = project.Properties.First(x => x.Name == "RootNamespace").EvaluatedValue;
                specFlowProject.ProjectSettings.ProjectPlatformSettings.Language = _languageReader.GetLanguage(project.FullPath);

                foreach (var item in project.FeatureFiles())
                {
                    var featureFile = specFlowProject.GetOrCreateFeatureFile(item.EvaluatedInclude);
                    var ns = item.GetMetadataValue("CustomToolNamespace");
                    if (!String.IsNullOrEmpty(ns))
                        featureFile.CustomNamespace = ns;
                    if (!specFlowProject.FeatureFiles.Contains(featureFile))
                    {
                        specFlowProject.FeatureFiles.Add(featureFile);
                    }
                }


                specFlowProject.ProjectSettings.ConfigurationHolder = GetSpecFlowConfigurationHolder(project, projectFolder);

                if (specFlowProject.ProjectSettings.ConfigurationHolder != null)
                {
                    specFlowProject.Configuration = _configurationLoader.LoadConfiguration(specFlowProject.ProjectSettings.ConfigurationHolder);
                }

                return specFlowProject;
            }
            catch (Exception e)
            {
                
                throw new Exception("Error when reading project file." + Environment.NewLine + "MSBuild Output: " + Environment.NewLine + logWriter + Environment.NewLine + "DebugInfo:" + Environment.NewLine + debugInfo, e);
            }
        }

        private SpecFlowConfigurationHolder GetSpecFlowConfigurationHolder(Microsoft.Build.Evaluation.Project project, string projectFolder)
        {
            var jsonConfig = project.SpecFlowJsonConfigurationFile();
            if (jsonConfig != null)
            {
                var configFilePath = Path.Combine(projectFolder, jsonConfig.EvaluatedInclude);
                var configFileContent = File.ReadAllText(configFilePath);
                return new SpecFlowConfigurationHolder(ConfigSource.Json, configFileContent);
            }
            else
            {
                var appConfigItem = project.ApplicationConfigurationFile();
                if (appConfigItem != null)
                {
                    var configFilePath = Path.Combine(projectFolder, appConfigItem.EvaluatedInclude);
                    var configFileContent = File.ReadAllText(configFilePath);
                    return GetConfigurationHolderFromFileContent(configFileContent);
                }
            }

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

            throw new Exception($"SpecFlow configuration could not be found in {projectFolder}");
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