using System;
using System.Collections.Generic;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContainerBuilder
    {
        IObjectContainer CreateGlobalContainer(IRuntimeConfigurationProvider configurationProvider = null);
        IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer);
    }

    public class ContainerBuilder : IContainerBuilder
    {
        public static IDefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        private readonly IDefaultDependencyProvider defaultDependencyProvider;

        public ContainerBuilder(IDefaultDependencyProvider defaultDependencyProvider = null)
        {
            this.defaultDependencyProvider = defaultDependencyProvider ?? DefaultDependencyProvider;
        }

        public virtual IObjectContainer CreateGlobalContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var container = new ObjectContainer();
            container.RegisterInstanceAs<IContainerBuilder>(this);

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

        public IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer)
        {
            var testThreadContainer = new ObjectContainer(globalContainer);

            defaultDependencyProvider.RegisterTestThreadContainerDefaults(testThreadContainer);

            var runtimePluginEvents = globalContainer.Resolve<RuntimePluginEvents>();
            runtimePluginEvents.RaiseCustomizeTestThreadDependencies(testThreadContainer);

            return testThreadContainer;
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
            var runtimePluginParameters = new RuntimePluginParameters
            {
                Parameters = pluginDescriptor.Parameters
            }; 
            plugin.Initialize(runtimePluginEvents, runtimePluginParameters);
        }

        protected virtual void RegisterDefaults(ObjectContainer container)
        {
            defaultDependencyProvider.RegisterGlobalContainerDefaults(container);
        }
    }
}
