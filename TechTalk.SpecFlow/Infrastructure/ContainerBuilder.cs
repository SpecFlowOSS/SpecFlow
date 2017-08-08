using System;
using System.Collections.Generic;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContainerBuilder
    {
        IObjectContainer CreateGlobalContainer(IRuntimeConfigurationProvider configurationProvider = null);
        IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer);
        IObjectContainer CreateScenarioContainer(IObjectContainer testThreadContainer, ScenarioInfo scenarioInfo);
        IObjectContainer CreateFeatureContainer(IObjectContainer testThreadContainer, FeatureInfo featureInfo);
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

            container.RegisterTypeAs<RuntimePluginEvents, RuntimePluginEvents>(); //NOTE: we need this unnecessary registration, due to a bug in BoDi (does not inherit non-registered objects)
            var runtimePluginEvents = container.Resolve<RuntimePluginEvents>();

            SpecFlowConfiguration specFlowConfiguration = ConfigurationLoader.GetDefault();
            specFlowConfiguration = configurationProvider.LoadConfiguration(specFlowConfiguration);

            LoadPlugins(configurationProvider, container, runtimePluginEvents, specFlowConfiguration);

            runtimePluginEvents.RaiseConfigurationDefaults(specFlowConfiguration);

            runtimePluginEvents.RaiseRegisterGlobalDependencies(container);

            if (specFlowConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(specFlowConfiguration.CustomDependencies);

            container.RegisterInstanceAs(specFlowConfiguration);

            if (specFlowConfiguration.UnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(specFlowConfiguration.UnitTestProvider));

            runtimePluginEvents.RaiseCustomizeGlobalDependencies(container, specFlowConfiguration);

            container.Resolve<IConfigurationLoader>().TraceConfigSource(container.Resolve<ITraceListener>(), specFlowConfiguration);

            return container;
        }

       

        public virtual IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer)
        {
            var testThreadContainer = new ObjectContainer(globalContainer);

            defaultDependencyProvider.RegisterTestThreadContainerDefaults(testThreadContainer);

            var runtimePluginEvents = globalContainer.Resolve<RuntimePluginEvents>();
            runtimePluginEvents.RaiseCustomizeTestThreadDependencies(testThreadContainer);
            testThreadContainer.Resolve<ITestObjectResolver>();
            return testThreadContainer;
        }

        public virtual IObjectContainer CreateScenarioContainer(IObjectContainer testThreadContainer, ScenarioInfo scenarioInfo)
        {
            if (testThreadContainer == null)
                throw new ArgumentNullException(nameof(testThreadContainer));

            var scenarioContainer = new ObjectContainer(testThreadContainer);
            scenarioContainer.RegisterInstanceAs(scenarioInfo);

            scenarioContainer.ObjectCreated += obj =>
            {
                var containerDependentObject = obj as IContainerDependentObject;
                if (containerDependentObject != null)
                    containerDependentObject.SetObjectContainer(scenarioContainer);
            };

            var runtimePluginEvents = testThreadContainer.Resolve<RuntimePluginEvents>();
            runtimePluginEvents.RaiseCustomizeScenarioDependencies(scenarioContainer);

            return scenarioContainer;
        }

        public IObjectContainer CreateFeatureContainer(IObjectContainer testThreadContainer, FeatureInfo featureInfo)
        {
            if (testThreadContainer == null)
                throw new ArgumentNullException(nameof(testThreadContainer));

            var featureContainer = new ObjectContainer(testThreadContainer);
            featureContainer.RegisterInstanceAs(featureInfo);

            return featureContainer;
        }

        protected virtual void LoadPlugins(IRuntimeConfigurationProvider configurationProvider, ObjectContainer container, RuntimePluginEvents runtimePluginEvents, SpecFlowConfiguration specFlowConfiguration)
        {
            // initialize plugins that were registered from code
            foreach (var runtimePlugin in container.Resolve<IDictionary<string, IRuntimePlugin>>().Values)
            {
                // these plugins cannot have parameters
                runtimePlugin.Initialize(runtimePluginEvents, new RuntimePluginParameters());
            }

            // load & initalize parameters from configuration
            var pluginLoader = container.Resolve<IRuntimePluginLoader>();
            foreach (var pluginDescriptor in configurationProvider.GetPlugins(specFlowConfiguration).Where(pd => (pd.Type & PluginType.Runtime) != 0))
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
