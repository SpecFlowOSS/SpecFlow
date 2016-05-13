using System;
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

            var runtimePluginEvents = container.Resolve<RuntimePluginEvents>();
            LoadPlugins(configurationProvider, container, runtimePluginEvents);

            runtimePluginEvents.RaiseRegisterGlobalDependencies(container);

            RuntimeConfiguration runtimeConfiguration = new RuntimeConfiguration();

            runtimePluginEvents.RaiseConfigurationDefaults(runtimeConfiguration);

            configurationProvider.LoadConfiguration(runtimeConfiguration);

#if !BODI_LIMITEDRUNTIME
            if (runtimeConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(runtimeConfiguration.CustomDependencies);
#endif

            container.RegisterInstanceAs(runtimeConfiguration);

            if (runtimeConfiguration.RuntimeUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProvider));

            runtimePluginEvents.RaiseCustomizeGlobalDependencies(container, runtimeConfiguration);

            return container;
        }

        public IObjectContainer CreateTestRunnerContainer(IObjectContainer globalContainer)
        {
            var testRunnerContainer = new ObjectContainer(globalContainer);

            defaultDependencyProvider.RegisterTestRunnerDefaults(testRunnerContainer);

            var runtimePluginEvents = globalContainer.Resolve<RuntimePluginEvents>();
            runtimePluginEvents.RaiseCustomizeTestRunnerDependencies(testRunnerContainer);

            return testRunnerContainer;
        }

        protected virtual void LoadPlugins(IRuntimeConfigurationProvider configurationProvider, ObjectContainer container, RuntimePluginEvents runtimePluginEvents)
        {
            // initialize plugins that were registered from code
            foreach (var runtimePlugin in container.Resolve<IDictionary<string, IRuntimePlugin>>().Values)
            {
                // these plugins cannot have parameters
                runtimePlugin.Initialize(runtimePluginEvents, new RuntimePluginParameters());
            }

            // load & initalize parameters from configuration
            var pluginLoader = container.Resolve<IRuntimePluginLoader>();
            foreach (var pluginDescriptor in configurationProvider.GetPlugins().Where(pd => (pd.Type & PluginType.Runtime) != 0))
            {
                LoadPlugin(pluginDescriptor, pluginLoader, runtimePluginEvents);
            }
        }

        protected virtual void LoadPlugin(PluginDescriptor pluginDescriptor, IRuntimePluginLoader pluginLoader, RuntimePluginEvents runtimePluginEvents)
        {
            var plugin = pluginLoader.LoadPlugin(pluginDescriptor);
            var runtimePluginParameters = new RuntimePluginParameters(); //TODO: populate parameter from pluginDescriptor
            plugin.Initialize(runtimePluginEvents, runtimePluginParameters);
        }

        protected virtual void RegisterDefaults(ObjectContainer container)
        {
            defaultDependencyProvider.RegisterDefaults(container);
        }
    }
}
