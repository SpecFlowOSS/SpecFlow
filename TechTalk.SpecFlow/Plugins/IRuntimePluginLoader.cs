using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginLoader
    {
        IRuntimePlugin LoadPlugin(string pluginAssemblyName);
    }
}