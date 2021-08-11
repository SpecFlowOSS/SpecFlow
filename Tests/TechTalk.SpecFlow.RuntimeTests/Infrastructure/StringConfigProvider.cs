using System;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Configuration.JsonConfig;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    internal class StringConfigProvider : IRuntimeConfigurationProvider
    {
        private readonly string configFileContent;

        private bool IsJsonConfig => configFileContent != null && configFileContent.StartsWith("{");

        public StringConfigProvider(string configContent)
        {
            configFileContent = configContent;
        }

        public SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration)
        {
            if (IsJsonConfig)
            {
                var jsonConfigurationLoader = new JsonConfigurationLoader();

                return jsonConfigurationLoader.LoadJson(specFlowConfiguration, configFileContent);
            }

            ConfigurationSectionHandler section = GetSection();

            var runtimeConfigurationLoader = new AppConfigConfigurationLoader();

            return runtimeConfigurationLoader.LoadAppConfig(specFlowConfiguration, section);
        }

        private ConfigurationSectionHandler GetSection()
        {
            XmlDocument configDocument = new XmlDocument();
            configDocument.LoadXml(configFileContent);

            var specFlowNode = configDocument.SelectSingleNode("/configuration/specFlow");
            if (specFlowNode == null)
                throw new InvalidOperationException("invalid config file content");

            return ConfigurationSectionHandler.CreateFromXml(specFlowNode);
        }
    }
}