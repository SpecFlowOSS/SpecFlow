using System.Collections.Generic;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public interface IRuntimeConfigurationProvider
    {
        SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration);
        IEnumerable<PluginDescriptor> GetPlugins(SpecFlowConfiguration specFlowConfiguration);
    }
}
