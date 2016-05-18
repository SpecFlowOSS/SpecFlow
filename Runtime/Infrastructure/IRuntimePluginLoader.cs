using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IRuntimePluginLoader
    {
        IRuntimePlugin LoadPlugin(PluginDescriptor pluginDescriptor);
    }

    public class RuntimePluginLoader : IRuntimePluginLoader
    {
        private const string ASSEMBLY_NAME_PATTERN = "{0}.SpecFlowPlugin";

        public IRuntimePlugin LoadPlugin(PluginDescriptor pluginDescriptor)
        {
            var assemblyName = string.Format(ASSEMBLY_NAME_PATTERN, pluginDescriptor.Name);
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch(Exception ex)
            {
                throw new SpecFlowException(string.Format("Unable to load plugin: {0}. Please check http://go.specflow.org/doc-plugins for details.", pluginDescriptor.Name), ex);
            }

            var pluginAttribute = (RuntimePluginAttribute)Attribute.GetCustomAttribute(assembly, typeof(RuntimePluginAttribute));
            if (pluginAttribute == null)
                throw new SpecFlowException(string.Format("Missing [assembly:RuntimePlugin] attribute in {0}. Please check http://go.specflow.org/doc-plugins for details.", assembly.FullName));

            if (!typeof(IRuntimePlugin).IsAssignableFrom((pluginAttribute.PluginType)))
                throw new SpecFlowException(string.Format("Invalid plugin attribute in {0}. Plugin type must implement IRuntimePlugin. Please check http://go.specflow.org/doc-plugins for details.", assembly.FullName));

            IRuntimePlugin plugin;
            try
            {
                plugin = (IRuntimePlugin)Activator.CreateInstance(pluginAttribute.PluginType);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException(string.Format("Invalid plugin in {0}. Plugin must have a default constructor that does not throw exception. Please check http://go.specflow.org/doc-plugins for details.", assembly.FullName), ex);
            }

            return plugin;
        }
    }
}