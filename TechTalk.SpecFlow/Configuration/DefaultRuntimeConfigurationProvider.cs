using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.PlatformSpecific;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public class DefaultRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        public RuntimeConfiguration LoadConfiguration(RuntimeConfiguration runtimeConfiguration)
        {
            var runtimeConfigurationLoader = new RuntimeConfigurationLoader();
            return runtimeConfigurationLoader.Load(runtimeConfiguration);
        }

        public IEnumerable<PluginDescriptor> GetPlugins()
        {
            return RuntimeConfiguration.GetPlugins();
        }
    }
}