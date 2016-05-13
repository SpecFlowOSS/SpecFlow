using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    public class GeneratorContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        public static IObjectContainer CreateContainer(SpecFlowConfigurationHolder configurationHolder, ProjectSettings projectSettings)
        {
            var container = new ObjectContainer();
            container.RegisterInstanceAs(projectSettings);

            RegisterDefaults(container);

            var configurationProvider = container.Resolve<IGeneratorConfigurationProvider>();

            var generatorPluginEvents = container.Resolve<GeneratorPluginEvents>();
            LoadPlugins(container, configurationProvider, configurationHolder, generatorPluginEvents);

            generatorPluginEvents.RaiseRegisterDependencies(container);

            var specFlowConfiguration = new SpecFlowProjectConfiguration();

            generatorPluginEvents.RaiseConfigurationDefaults(specFlowConfiguration);

            configurationProvider.LoadConfiguration(configurationHolder, specFlowConfiguration);

            if (specFlowConfiguration.GeneratorConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(specFlowConfiguration.GeneratorConfiguration.CustomDependencies);

            container.RegisterInstanceAs(specFlowConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.GeneratorConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.RuntimeConfiguration);

            var generatorInfo = container.Resolve<IGeneratorInfoProvider>().GetGeneratorInfo();
            container.RegisterInstanceAs(generatorInfo);

            container.RegisterInstanceAs(container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language));

            if (specFlowConfiguration.GeneratorConfiguration.GeneratorUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestGeneratorProvider>(specFlowConfiguration.GeneratorConfiguration.GeneratorUnitTestProvider));

            generatorPluginEvents.RaiseCustomizeDependencies(container, specFlowConfiguration);

            return container;
        }

        private static void LoadPlugins(ObjectContainer container, IGeneratorConfigurationProvider configurationProvider, SpecFlowConfigurationHolder configurationHolder, GeneratorPluginEvents generatorPluginEvents)
        {
            // initialize plugins that were registered from code
            foreach (var generatorPlugin in container.Resolve<IDictionary<string, IGeneratorPlugin>>().Values)
            {
                // these plugins cannot have parameters
                generatorPlugin.Initialize(generatorPluginEvents, new GeneratorPluginParameters());
            }

            var pluginLoader = container.Resolve<IGeneratorPluginLoader>();
            foreach (var pluginDescriptor in configurationProvider.GetPlugins(configurationHolder).Where(pd => (pd.Type & PluginType.Generator) != 0))
            {
                LoadPlugin(pluginLoader, pluginDescriptor, generatorPluginEvents);
            }
        }

        private static void LoadPlugin(IGeneratorPluginLoader pluginLoader, PluginDescriptor pluginDescriptor, GeneratorPluginEvents generatorPluginEvents)
        {
            var plugin = pluginLoader.LoadPlugin(pluginDescriptor);
            var generatorPluginParameters = new GeneratorPluginParameters(); //TODO: populate parameter from pluginDescriptor
            plugin.Initialize(generatorPluginEvents, generatorPluginParameters);
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }
}