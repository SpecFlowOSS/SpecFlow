using MiniDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;

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

            if (runtimeConfiguration.TraceListenerType != null)
                container.RegisterTypeAs<ITraceListener>(runtimeConfiguration.TraceListenerType);

            return container;
        }

        private static void RegisterDefaults(MiniDi.ObjectContainer container)
        {
            container.RegisterTypeAs<TestRunnerFactory, ITestRunnerFactory>();
            container.RegisterTypeAs<TestRunner, ITestRunner>();

            container.RegisterTypeAs<StepFormatter, IStepFormatter>();
            container.RegisterTypeAs<TestTracer, ITestTracer>();

            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<ErrorProvider, IErrorProvider>();
            container.RegisterTypeAs<StepArgumentTypeConverter, IStepArgumentTypeConverter>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
        }
    }
}
