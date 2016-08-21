﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.PlatformSpecific;
using TechTalk.SpecFlow.PlatformSpecific.AppConfig;
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

        public RuntimeConfiguration LoadConfiguration(RuntimeConfiguration runtimeConfiguration)
        {
            ConfigurationSectionHandler section = GetSection();
            
            var runtimeConfigurationLoader = new AppConfigConfigurationLoader();

            return runtimeConfigurationLoader.LoadAppConfig(runtimeConfiguration, section);

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