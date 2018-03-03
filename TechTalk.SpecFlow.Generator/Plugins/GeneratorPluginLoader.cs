using System;
using System.Reflection;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public class GeneratorPluginLoader : IGeneratorPluginLoader
    {
        private readonly ProjectSettings projectSettings;
        private readonly IGeneratorPluginLocator generatorPluginLocator;

        public GeneratorPluginLoader(ProjectSettings projectSettings, IGeneratorPluginLocator generatorPluginLocator)
        {
            this.projectSettings = projectSettings;
            this.generatorPluginLocator = generatorPluginLocator;
        }

        public IGeneratorPlugin LoadPlugin(PluginDescriptor pluginDescriptor)
        {
            var generatorPluginAssemblyPath = this.generatorPluginLocator.LocatePluginAssembly(pluginDescriptor);

            Assembly pluginAssembly;
            try
            {
                pluginAssembly = Assembly.LoadFrom(generatorPluginAssemblyPath);
            }
            catch(Exception ex)
            {
                throw new SpecFlowException($"Unable to load plugin assembly: {generatorPluginAssemblyPath}. Please check http://go.specflow.org/doc-plugins for details.", ex);
            }

            var pluginAttribute = (GeneratorPluginAttribute)Attribute.GetCustomAttribute(pluginAssembly, typeof(GeneratorPluginAttribute));
            if (pluginAttribute == null)
                throw new SpecFlowException("Missing [assembly:GeneratorPlugin] attribute in " + generatorPluginAssemblyPath);

            if (!typeof(IGeneratorPlugin).IsAssignableFrom((pluginAttribute.PluginType)))
                throw new SpecFlowException($"Invalid plugin attribute in {generatorPluginAssemblyPath}. Plugin type must implement IGeneratorPlugin. Please check http://go.specflow.org/doc-plugins for details.");

            IGeneratorPlugin plugin;
            try
            {
                plugin = (IGeneratorPlugin)Activator.CreateInstance(pluginAttribute.PluginType);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException($"Invalid plugin in {generatorPluginAssemblyPath}. Plugin must have a default constructor that does not throw exception. Please check http://go.specflow.org/doc-plugins for details.", ex);
            }

            return plugin;
        }
    }
}