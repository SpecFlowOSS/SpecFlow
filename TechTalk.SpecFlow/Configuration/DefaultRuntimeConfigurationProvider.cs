using System.Collections.Generic;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public class DefaultRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        private readonly IConfigurationLoader _configurationLoader;

        public DefaultRuntimeConfigurationProvider(IConfigurationLoader configurationLoader)
        {
            _configurationLoader = configurationLoader;
        }

        public SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration)
        {            
            return _configurationLoader.Load(specFlowConfiguration);
        }

        public IEnumerable<PluginDescriptor> GetPlugins(SpecFlowConfiguration specFlowConfiguration)
        {
            return LoadConfiguration(specFlowConfiguration).Plugins;
        }
    }
}