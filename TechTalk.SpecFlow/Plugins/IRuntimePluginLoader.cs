using System;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginLoader
    {
        IRuntimePlugin LoadPlugin(PluginDescriptor pluginDescriptor);
    }
}