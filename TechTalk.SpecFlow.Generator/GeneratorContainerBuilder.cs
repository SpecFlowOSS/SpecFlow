using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
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

            var specFlowConfiguration = new SpecFlowProjectConfiguration();
            specFlowConfiguration.SpecFlowConfiguration = configurationProvider.LoadConfiguration(specFlowConfiguration.SpecFlowConfiguration, configurationHolder);

            LoadPlugins(container, configurationProvider, configurationHolder, generatorPluginEvents, specFlowConfiguration);

            generatorPluginEvents.RaiseRegisterDependencies(container);
            generatorPluginEvents.RaiseConfigurationDefaults(specFlowConfiguration);
            
            if (specFlowConfiguration.SpecFlowConfiguration.GeneratorCustomDependencies != null)
                container.RegisterFromConfiguration(specFlowConfiguration.SpecFlowConfiguration.GeneratorCustomDependencies);

            container.RegisterInstanceAs(specFlowConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.SpecFlowConfiguration);

            var generatorInfo = container.Resolve<IGeneratorInfoProvider>().GetGeneratorInfo();
            container.RegisterInstanceAs(generatorInfo);

            container.RegisterInstanceAs(container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language));

            if (specFlowConfiguration.SpecFlowConfiguration.UnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestGeneratorProvider>(specFlowConfiguration.SpecFlowConfiguration.UnitTestProvider));

            generatorPluginEvents.RaiseCustomizeDependencies(container, specFlowConfiguration);

            container.Resolve<IConfigurationLoader>().TraceConfigSource(container.Resolve<ITraceListener>(), specFlowConfiguration.SpecFlowConfiguration);


            return container;
        }

        private static void LoadPlugins(ObjectContainer container, IGeneratorConfigurationProvider configurationProvider, SpecFlowConfigurationHolder configurationHolder, GeneratorPluginEvents generatorPluginEvents, SpecFlowProjectConfiguration specFlowConfiguration)
        {
            // initialize plugins that were registered from code
            foreach (var generatorPlugin in container.Resolve<IDictionary<string, IGeneratorPlugin>>().Values)
            {
                // these plugins cannot have parameters
                generatorPlugin.Initialize(generatorPluginEvents, new GeneratorPluginParameters());
            }

            var pluginLoader = container.Resolve<IGeneratorPluginLoader>();
            foreach (var pluginDescriptor in configurationProvider.GetPlugins(specFlowConfiguration.SpecFlowConfiguration, configurationHolder).Where(pd => (pd.Type & PluginType.Generator) != 0))
            {
                LoadPlugin(pluginDescriptor, pluginLoader, generatorPluginEvents);
            }
        }

        private static void LoadPlugin(PluginDescriptor pluginDescriptor, IGeneratorPluginLoader pluginLoader, GeneratorPluginEvents generatorPluginEvents)
        {
            var plugin = pluginLoader.LoadPlugin(pluginDescriptor);
            var generatorPluginParameters = new GeneratorPluginParameters
            {
                Parameters = pluginDescriptor.Parameters
            };
            plugin.Initialize(generatorPluginEvents, generatorPluginParameters);
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }
}