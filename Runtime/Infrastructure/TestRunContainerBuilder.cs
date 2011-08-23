using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class TestRunContainerBuilder
    {
        public static IObjectContainer CreateContainer()
        {
            var container = new BoDi.ObjectContainer();

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

            if (runtimeConfiguration.RuntimeUnitTestProviderType != null)
                container.RegisterTypeAs<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProviderType);

            return container;
        }

        private static void RegisterDefaults(BoDi.ObjectContainer container)
        {
            container.RegisterTypeAs<TestRunnerFactory, ITestRunnerFactory>();
            container.RegisterTypeAs<TestRunner, ITestRunner>();

            container.RegisterTypeAs<StepFormatter, IStepFormatter>();
            container.RegisterTypeAs<TestTracer, ITestTracer>();

            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<ErrorProvider, IErrorProvider>();
            container.RegisterTypeAs<StepArgumentTypeConverter, IStepArgumentTypeConverter>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
            container.RegisterTypeAs<BindingFactory, IBindingFactory>();

            container.RegisterTypeAs<NUnitRuntimeProvider, IUnitTestRuntimeProvider>();

            container.RegisterTypeAs<ContextManager, IContextManager>();

            //this part will be changed for proper named registration support
            IDictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider> stepDefinitionSkeletonProviders =
                new Dictionary<ProgrammingLanguage, IStepDefinitionSkeletonProvider>
                    {
                        { ProgrammingLanguage.CSharp, new StepDefinitionSkeletonProviderCS() },
                        { ProgrammingLanguage.VB, new StepDefinitionSkeletonProviderVB() },
                    };
            container.RegisterInstanceAs(stepDefinitionSkeletonProviders);
        }
    }
}
