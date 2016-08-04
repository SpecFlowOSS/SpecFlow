using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public interface IRuntimeConfigurationProvider
    {
        RuntimeConfiguration LoadConfiguration(RuntimeConfiguration runtimeConfiguration);
        IEnumerable<PluginDescriptor> GetPlugins();
    }
}
