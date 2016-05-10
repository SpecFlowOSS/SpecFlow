using BoDi;
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

            var runtimePlugins = LoadPlugins(configurationProvider, container);
            container.RegisterInstanceAs(runtimePlugins, typeof(IRuntimePlugins));


            foreach (var plugin in runtimePlugins.LoadedRuntimePlugins)
                plugin.RuntimePluginEvents.RaiseRegisterGlobalDependencies(container);

            RuntimeConfiguration runtimeConfiguration = new RuntimeConfiguration();

            foreach (var plugin in runtimePlugins.LoadedRuntimePlugins)
                plugin.RuntimePluginEvents.RaiseConfigurationDefaults(runtimeConfiguration);

            configurationProvider.LoadConfiguration(runtimeConfiguration);

#if !BODI_LIMITEDRUNTIME
            if (runtimeConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(runtimeConfiguration.CustomDependencies);
#endif

            container.RegisterInstanceAs(runtimeConfiguration);

            if (runtimeConfiguration.RuntimeUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProvider));

            foreach (var plugin in runtimePlugins.LoadedRuntimePlugins)
                plugin.RuntimePluginEvents.RaiseCustomizeGlobalDependencies(container, runtimeConfiguration);

            return container;
        }

        public IObjectContainer CreateTestRunnerContainer(IObjectContainer globalContainer)
        {
            var runtimePlugins = globalContainer.Resolve<IRuntimePlugins>();

            var testRunnerContainer = new ObjectContainer(globalContainer);

            defaultDependencyProvider.RegisterTestRunnerDefaults(testRunnerContainer);

            foreach (var runtimePlugin in runtimePlugins.LoadedRuntimePlugins)
            {
                runtimePlugin.RuntimePluginEvents.RaiseCustomizeTestRunnerDependencies(testRunnerContainer);
            }

            return testRunnerContainer;
        }

        protected virtual IRuntimePlugins LoadPlugins(IRuntimeConfigurationProvider configurationProvider, ObjectContainer container)
        {
            var runtimePluginLoader = container.Resolve<IRuntimePluginsLoader>();

            return runtimePluginLoader.LoadRuntimePlugins(configurationProvider);
        }

        protected virtual void RegisterDefaults(ObjectContainer container)
        {
            defaultDependencyProvider.RegisterDefaults(container);
        }
    }
}
