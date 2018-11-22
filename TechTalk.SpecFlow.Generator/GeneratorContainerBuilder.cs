using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    public class GeneratorContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        public static IObjectContainer CreateContainer(SpecFlowConfigurationHolder configurationHolder, ProjectSettings projectSettings, IEnumerable<string> generatorPlugins)
        {
            var container = new ObjectContainer();
            container.RegisterInstanceAs(projectSettings);

            RegisterDefaults(container);

            var configurationProvider = container.Resolve<IGeneratorConfigurationProvider>();
            var generatorPluginEvents = container.Resolve<GeneratorPluginEvents>();
            var unitTestProviderConfigration = container.Resolve<UnitTestProviderConfiguration>();

            var specFlowConfiguration = new SpecFlowProjectConfiguration();
            specFlowConfiguration.SpecFlowConfiguration = configurationProvider.LoadConfiguration(specFlowConfiguration.SpecFlowConfiguration, configurationHolder);

            LoadPlugins(container, generatorPluginEvents, unitTestProviderConfigration, generatorPlugins);
            
            generatorPluginEvents.RaiseRegisterDependencies(container);
            generatorPluginEvents.RaiseConfigurationDefaults(specFlowConfiguration);
            
            if (specFlowConfiguration.SpecFlowConfiguration.GeneratorCustomDependencies != null)
                container.RegisterFromConfiguration(specFlowConfiguration.SpecFlowConfiguration.GeneratorCustomDependencies);

            container.RegisterInstanceAs(specFlowConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.SpecFlowConfiguration);

            var generatorInfo = container.Resolve<IGeneratorInfoProvider>().GetGeneratorInfo();
            container.RegisterInstanceAs(generatorInfo);

            container.RegisterInstanceAs(container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language));

            if (unitTestProviderConfigration != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestGeneratorProvider>(unitTestProviderConfigration.UnitTestProvider ?? ConfigDefaults.UnitTestProviderName));

            generatorPluginEvents.RaiseCustomizeDependencies(container, specFlowConfiguration);

            container.Resolve<IConfigurationLoader>().TraceConfigSource(container.Resolve<ITraceListener>(), specFlowConfiguration.SpecFlowConfiguration);


            return container;
        }

        private static void LoadPlugins(ObjectContainer container, GeneratorPluginEvents generatorPluginEvents, UnitTestProviderConfiguration unitTestProviderConfigration, IEnumerable<string> generatorPlugins)
        {
            // initialize plugins that were registered from code
            foreach (var generatorPlugin in container.Resolve<IDictionary<string, IGeneratorPlugin>>().Values)
            {
                // these plugins cannot have parameters
                generatorPlugin.Initialize(generatorPluginEvents, new GeneratorPluginParameters(), unitTestProviderConfigration);
            }
            var pluginLoader = container.Resolve<IGeneratorPluginLoader>();

            foreach (var generatorPlugin in generatorPlugins)
            {
                //todo: should set the parameters, and do not pass empty
                var pluginDescriptor = new PluginDescriptor(Path.GetFileNameWithoutExtension(generatorPlugin), generatorPlugin, PluginType.Generator, string.Empty); 
                LoadPlugin(pluginDescriptor, pluginLoader, generatorPluginEvents, unitTestProviderConfigration);
            }
        }

        private static void LoadPlugin(PluginDescriptor pluginDescriptor, IGeneratorPluginLoader pluginLoader, GeneratorPluginEvents generatorPluginEvents,
            UnitTestProviderConfiguration unitTestProviderConfigration)
        {
            var plugin = pluginLoader.LoadPlugin(pluginDescriptor);
            var generatorPluginParameters = new GeneratorPluginParameters
            {
                Parameters = pluginDescriptor.Parameters
            };
            plugin.Initialize(generatorPluginEvents, generatorPluginParameters, unitTestProviderConfigration);
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }
}