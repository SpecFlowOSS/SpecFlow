using System;
using System.Reflection;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginLoader
    {
        IRuntimePlugin LoadPlugin(string pluginAssemblyName, ITraceListener traceListener);
    }
}