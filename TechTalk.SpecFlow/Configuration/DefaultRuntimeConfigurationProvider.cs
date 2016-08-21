using System.Collections.Generic;
using TechTalk.SpecFlow.PlatformSpecific;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public class DefaultRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        private readonly IRuntimeConfigurationLoader _runtimeConfigurationLoader;

        public DefaultRuntimeConfigurationProvider(IRuntimeConfigurationLoader runtimeConfigurationLoader)
        {
            _runtimeConfigurationLoader = runtimeConfigurationLoader;
        }

        public RuntimeConfiguration LoadConfiguration(RuntimeConfiguration runtimeConfiguration)
        {            
            return _runtimeConfigurationLoader.Load(runtimeConfiguration);
        }

        public IEnumerable<PluginDescriptor> GetPlugins()
        {
            return RuntimeConfiguration.GetPlugins();
        }
    }
}