using MiniDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class TestRunContainerBuilder
    {
        public static IObjectContainer CreateContainer()
        {
            var container = new MiniDi.ObjectContainer();

            RegisterDefaults(container);

// TODO: do somethign like this
//            var specFlowConfiguration = container.Resolve<ISpecFlowProjectConfigurationLoader>()
//                .LoadConfiguration(configurationHolder);
//
//            if (specFlowConfiguration.GeneratorConfiguration.CustomDependencies != null)
//                container.RegisterFromConfiguration(specFlowConfiguration.GeneratorConfiguration.CustomDependencies);
//            container.RegisterInstanceAs(specFlowConfiguration);
//            container.RegisterInstanceAs(specFlowConfiguration.GeneratorConfiguration);
//            container.RegisterInstanceAs(specFlowConfiguration.RuntimeConfiguration);

            var runtimeConfiguration = RuntimeConfiguration.GetConfig();
            container.RegisterInstanceAs(runtimeConfiguration);

            return container;
        }

        private static void RegisterDefaults(MiniDi.ObjectContainer container)
        {
            container.RegisterTypeAs<TestRunnerFactory, ITestRunnerFactory>();
            container.RegisterTypeAs<TestRunner, ITestRunner>();
        }
    }
}
