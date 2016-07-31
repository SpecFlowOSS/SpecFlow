using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public class DefaultRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        public void LoadConfiguration(RuntimeConfiguration defaultConfiguration)
        {
            defaultConfiguration.LoadConfiguration();
        }

        public IEnumerable<PluginDescriptor> GetPlugins()
        {
            return RuntimeConfiguration.GetPlugins();
        }
    }
}