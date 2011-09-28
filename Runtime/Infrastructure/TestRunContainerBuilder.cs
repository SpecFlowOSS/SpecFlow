using System;
using System.Collections.Generic;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class TestRunContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        public static IObjectContainer CreateContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var container = new ObjectContainer();

            RegisterDefaults(container);

            if (configurationProvider != null)
                container.RegisterInstanceAs(configurationProvider);

            configurationProvider = configurationProvider ?? container.Resolve<IRuntimeConfigurationProvider>();

            var pluginLoader = container.Resolve<IRuntimePluginLoader>();
            var plugins = configurationProvider.GetPlugins().Select(pd => LoadPlugin(pluginLoader, pd)).ToArray();
            foreach (var plugin in plugins)
            {
                plugin.RegisterDefaults(container);
            }

            RuntimeConfiguration runtimeConfiguration = new RuntimeConfiguration();
            foreach (var defaultsProvider in container.Resolve<IDictionary<string, IRuntimeConfigurationDefaultsProvider>>().Values)
            {
                defaultsProvider.SetDefaultConfiguration(runtimeConfiguration);
            }
            configurationProvider.LoadConfiguration(runtimeConfiguration);

#if !BODI_LIMITEDRUNTIME
            if (runtimeConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(runtimeConfiguration.CustomDependencies);
#endif

            container.RegisterInstanceAs(runtimeConfiguration);

            if (runtimeConfiguration.TraceListenerType != null)
                container.RegisterTypeAs<ITraceListener>(runtimeConfiguration.TraceListenerType);

            if (runtimeConfiguration.RuntimeUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProvider));

            foreach (var plugin in plugins)
            {
                plugin.RegisterCustomizations(container, runtimeConfiguration);
            }

            return container;
        }

        private static IRuntimePlugin LoadPlugin(IRuntimePluginLoader pluginLoader, PluginDescriptor pluginDescriptor)
        {
            return pluginLoader.LoadRuntimePlugin(pluginDescriptor.Name);
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }
}
