using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    internal class StringConfigProvider : IRuntimeConfigurationProvider
    {
        private readonly string configFileContent;

        public StringConfigProvider(string configContent)
        {
            this.configFileContent = configContent;
        }

        public void LoadConfiguration(RuntimeConfiguration defaultConfiguration)
        {
            ConfigurationSectionHandler section = GetSection();
            defaultConfiguration.LoadConfiguration(section);
        }

        public IEnumerable<PluginDescriptor> GetPlugins()
        {
            ConfigurationSectionHandler section = GetSection();
            if (section == null || section.Plugins == null)
                return Enumerable.Empty<PluginDescriptor>();

            return section.Plugins.Select(pce => pce.ToPluginDescriptor());
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