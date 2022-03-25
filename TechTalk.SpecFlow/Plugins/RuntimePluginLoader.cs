using System;
using System.Reflection;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Plugins
{
    public class RuntimePluginLoader : IRuntimePluginLoader
    {
        public IRuntimePlugin LoadPlugin(string pluginAssemblyName, ITraceListener traceListener, bool traceMissingPluginAttribute)
        {
            Assembly assembly;
            try
            {
#if NETSTANDARD
                assembly = PluginAssemblyResolver.Load(pluginAssemblyName);
#else
                assembly = Assembly.LoadFrom(pluginAssemblyName);
#endif
            }
            catch (Exception ex)
            {
                throw new SpecFlowException($"Unable to load plugin: {pluginAssemblyName}. Please check https://go.specflow.org/doc-plugins for details.", ex);
            }

            var pluginAttribute = (RuntimePluginAttribute)Attribute.GetCustomAttribute(assembly, typeof(RuntimePluginAttribute));
            if (pluginAttribute == null)
            {
                if (traceMissingPluginAttribute)
                    traceListener.WriteToolOutput($"Missing [assembly:RuntimePlugin] attribute in {assembly.FullName}. Please check https://go.specflow.org/doc-plugins for details.");

                return null;
            }

            if (!typeof(IRuntimePlugin).IsAssignableFrom(pluginAttribute.PluginType))
                throw new SpecFlowException($"Invalid plugin attribute in {assembly.FullName}. Plugin type must implement IRuntimePlugin. Please check https://go.specflow.org/doc-plugins for details.");

            IRuntimePlugin plugin;
            try
            {
                plugin = (IRuntimePlugin)Activator.CreateInstance(pluginAttribute.PluginType);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException($"Invalid plugin in {assembly.FullName}. Plugin must have a default constructor that does not throw exception. Please check https://go.specflow.org/doc-plugins for details.", ex);
            }

            return plugin;
        }
    }
}