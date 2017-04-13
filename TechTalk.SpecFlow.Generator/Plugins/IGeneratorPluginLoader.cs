using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPluginLoader
    {
        IGeneratorPlugin LoadPlugin(PluginDescriptor pluginDescriptor);
    }

    public class GeneratorPluginLoader : IGeneratorPluginLoader
    {         
        private readonly IGeneratorPluginLocator generatorPluginLocator;

        public GeneratorPluginLoader(IGeneratorPluginLocator generatorPluginLocator)
        {
            this.generatorPluginLocator = generatorPluginLocator;
        }

        public IGeneratorPlugin LoadPlugin(PluginDescriptor pluginDescriptor)
        {
            var generatorPluginAssemblyPath = this.generatorPluginLocator.GetGeneratorPluginAssemblies(pluginDescriptor).FirstOrDefault();
            if (generatorPluginAssemblyPath == null)
                throw new SpecFlowException(string.Format("Unable to find plugin in the plugin search path: {0}. Please check http://go.specflow.org/doc-plugins for details.", pluginDescriptor.Name));

            Assembly pluginAssembly;
            try
            {
                pluginAssembly = Assembly.LoadFrom(generatorPluginAssemblyPath);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException(string.Format("Unable to load plugin assembly: {0}. Please check http://go.specflow.org/doc-plugins for details.", generatorPluginAssemblyPath), ex);
            }

            var pluginAttribute = (GeneratorPluginAttribute)Attribute.GetCustomAttribute(pluginAssembly, typeof(GeneratorPluginAttribute));
            if (pluginAttribute == null)
                throw new SpecFlowException("Missing [assembly:GeneratorPlugin] attribute in " + generatorPluginAssemblyPath);

            if (!typeof(IGeneratorPlugin).IsAssignableFrom((pluginAttribute.PluginType)))
                throw new SpecFlowException(string.Format("Invalid plugin attribute in {0}. Plugin type must implement IGeneratorPlugin. Please check http://go.specflow.org/doc-plugins for details.", generatorPluginAssemblyPath));

            IGeneratorPlugin plugin;
            try
            {
                plugin = (IGeneratorPlugin)Activator.CreateInstance(pluginAttribute.PluginType);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException(string.Format("Invalid plugin in {0}. Plugin must have a default constructor that does not throw exception. Please check http://go.specflow.org/doc-plugins for details.", generatorPluginAssemblyPath), ex);
            }

            return plugin;
        }
    }
}
