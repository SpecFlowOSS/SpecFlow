using System;
using System.Reflection;
using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class RuntimePluginAttribute: Attribute
    {
        public Type PluginType { get; private set; }

        public RuntimePluginAttribute(Type pluginType)
        {
            if (pluginType == null) throw new ArgumentNullException("pluginType");

            PluginType = pluginType;
        }
    }

    public interface IRuntimePluginLoader
    {
        IRuntimePlugin LoadRuntimePlugin(string name);
    }

    public class RuntimePluginLoader : IRuntimePluginLoader
    {
        private const string ASSEMBLY_NAME_PATTERN = "{0}.SpecFlowPlugin";

        public IRuntimePlugin LoadRuntimePlugin(string name)
        {
            //TODO: support for loading plugins for a specific version: MyPlugin.SpecFlowPlugin.6.5.dll
            var assemblyName = string.Format(ASSEMBLY_NAME_PATTERN, name);
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch(Exception ex)
            {
                throw new SpecFlowException("Unable to load plugin: " + name, ex);
            }

            var pluginAttribute = (RuntimePluginAttribute)Attribute.GetCustomAttribute(assembly, typeof(RuntimePluginAttribute));
            if (pluginAttribute == null)
                throw new SpecFlowException("Missing plugin attribute in " + assembly.FullName);

            if (!typeof(IRuntimePlugin).IsAssignableFrom((pluginAttribute.PluginType)))
                throw new SpecFlowException("Invalid plugin attribute in " + assembly.FullName);

            IRuntimePlugin plugin;
            try
            {
                plugin = (IRuntimePlugin)Activator.CreateInstance(pluginAttribute.PluginType);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException("Invalid plugin in " + assembly.FullName, ex);
            }

            return plugin;
        }
    }
}