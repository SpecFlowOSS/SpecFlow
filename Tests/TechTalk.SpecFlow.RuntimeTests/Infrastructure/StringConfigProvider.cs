using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    internal class StringConfigProvider : IRuntimeConfigurationProvider
    {
        private readonly string configFileContent;

        public StringConfigProvider(string configContent)
        {
            this.configFileContent = configContent;
        }

        public SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration)
        {
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