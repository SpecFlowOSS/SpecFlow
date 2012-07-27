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

            var plugins = LoadPlugins(container, configurationProvider, configurationHolder);
            foreach (var plugin in plugins)
                plugin.RegisterDependencies(container);

            var specFlowConfiguration = new SpecFlowProjectConfiguration();

            foreach (var plugin in plugins)
                plugin.RegisterConfigurationDefaults(specFlowConfiguration);

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

            foreach (var plugin in plugins)
                plugin.RegisterCustomizations(container, specFlowConfiguration);

            return container;
        }

        private static IGeneratorPlugin[] LoadPlugins(ObjectContainer container, IGeneratorConfigurationProvider configurationProvider, SpecFlowConfigurationHolder configurationHolder)
        {
            var plugins = container.Resolve<IDictionary<string, IGeneratorPlugin>>().Values.AsEnumerable();

            var pluginLoader = container.Resolve<IGeneratorPluginLoader>();
            plugins = plugins.Concat(configurationProvider.GetPlugins(configurationHolder).Where(pd => (pd.Type & PluginType.Generator) != 0).Select(pd => LoadPlugin(pluginLoader, pd)));

            return plugins.ToArray();
        }

        private static IGeneratorPlugin LoadPlugin(IGeneratorPluginLoader pluginLoader, PluginDescriptor pluginDescriptor)
        {
            return pluginLoader.LoadPlugin(pluginDescriptor);
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }
}