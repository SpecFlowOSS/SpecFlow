﻿using System;
using System.Collections.Generic;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestRunContainerBuilder
    {
        IObjectContainer CreateContainer(IRuntimeConfigurationProvider configurationProvider = null);
        IObjectContainer CreateTestRunnerContainer(IObjectContainer globalContainer);
    }

    public class TestRunContainerBuilder : ITestRunContainerBuilder
    {
        public static IDefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        private readonly IDefaultDependencyProvider defaultDependencyProvider;

        public TestRunContainerBuilder(IDefaultDependencyProvider defaultDependencyProvider = null)
        {
            this.defaultDependencyProvider = defaultDependencyProvider ?? DefaultDependencyProvider;
        }

        public virtual IObjectContainer CreateContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var container = new ObjectContainer();
            container.RegisterInstanceAs<ITestRunContainerBuilder>(this);

            RegisterDefaults(container);

            if (configurationProvider != null)
                container.RegisterInstanceAs(configurationProvider);

            configurationProvider = configurationProvider ?? container.Resolve<IRuntimeConfigurationProvider>();

            var plugins = LoadPlugins(configurationProvider, container);
            foreach (var plugin in plugins)
                plugin.RegisterDependencies(container);

            RuntimeConfiguration runtimeConfiguration = new RuntimeConfiguration();

            foreach (var plugin in plugins)
                plugin.RegisterConfigurationDefaults(runtimeConfiguration);

            configurationProvider.LoadConfiguration(runtimeConfiguration);

#if !BODI_LIMITEDRUNTIME
            if (runtimeConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(runtimeConfiguration.CustomDependencies);
#endif

            container.RegisterInstanceAs(runtimeConfiguration);

            if (runtimeConfiguration.RuntimeUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProvider));

            foreach (var plugin in plugins)
                plugin.RegisterCustomizations(container, runtimeConfiguration);

            return container;
        }

        public IObjectContainer CreateTestRunnerContainer(IObjectContainer globalContainer)
        {
            var testRunnerContainer = new ObjectContainer(globalContainer);

            defaultDependencyProvider.RegisterTestRunnerDefaults(testRunnerContainer);

            var configurationProvider = testRunnerContainer.Resolve<IRuntimeConfigurationProvider>();
            var plugins = LoadPlugins(configurationProvider, testRunnerContainer);

            foreach (var plugin in plugins)
                plugin.CustomizeTestRunnerDependencies(testRunnerContainer);

            return testRunnerContainer;
        }

        protected virtual IRuntimePlugin[] LoadPlugins(IRuntimeConfigurationProvider configurationProvider, ObjectContainer container)
        {
            var plugins = container.Resolve<IDictionary<string, IRuntimePlugin>>().Values.AsEnumerable();

            var pluginLoader = container.Resolve<IRuntimePluginLoader>();
            plugins = plugins.Concat(configurationProvider.GetPlugins().Where(pd => (pd.Type & PluginType.Runtime) != 0).Select(pd => LoadPlugin(pluginLoader, pd)));

            return plugins.ToArray();
        }

        protected virtual IRuntimePlugin LoadPlugin(IRuntimePluginLoader pluginLoader, PluginDescriptor pluginDescriptor)
        {
            return pluginLoader.LoadPlugin(pluginDescriptor);
        }

        protected virtual void RegisterDefaults(ObjectContainer container)
        {
            defaultDependencyProvider.RegisterDefaults(container);
        }
    }
}
