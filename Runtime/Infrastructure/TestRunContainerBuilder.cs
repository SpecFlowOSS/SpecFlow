using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal partial class TestRunContainerBuilder
    {
        public static IObjectContainer CreateContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var container = new ObjectContainer();

            RegisterDefaults(container);

            if (configurationProvider != null)
                container.RegisterInstanceAs(configurationProvider);

            configurationProvider = configurationProvider ?? container.Resolve<IRuntimeConfigurationProvider>();
            var runtimeConfiguration = configurationProvider.GetConfiguration();

#if !BODI_LIMITEDRUNTIME
            if (runtimeConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(runtimeConfiguration.CustomDependencies);
#endif

            container.RegisterInstanceAs(runtimeConfiguration);

            if (runtimeConfiguration.TraceListenerType != null)
                container.RegisterTypeAs<ITraceListener>(runtimeConfiguration.TraceListenerType);

            if (runtimeConfiguration.RuntimeUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestRuntimeProvider>(runtimeConfiguration.RuntimeUnitTestProvider));

            return container;
        }

        static partial void RegisterUnitTestProviders(ObjectContainer container);

        private static void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<DefaultRuntimeConfigurationProvider, IRuntimeConfigurationProvider>();

            container.RegisterTypeAs<TestRunnerFactory, ITestRunnerFactory>();
            container.RegisterTypeAs<TestRunner, ITestRunner>();
            container.RegisterTypeAs<TestExecutionEngine, ITestExecutionEngine>();

            container.RegisterTypeAs<StepFormatter, IStepFormatter>();
            container.RegisterTypeAs<TestTracer, ITestTracer>();

            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<ErrorProvider, IErrorProvider>();
            container.RegisterTypeAs<StepArgumentTypeConverter, IStepArgumentTypeConverter>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
            container.RegisterTypeAs<BindingFactory, IBindingFactory>();

            container.RegisterTypeAs<ContextManager, IContextManager>();

            container.RegisterTypeAs<StepDefinitionSkeletonProviderCS, IStepDefinitionSkeletonProvider>(ProgrammingLanguage.CSharp.ToString());
            container.RegisterTypeAs<StepDefinitionSkeletonProviderVB, IStepDefinitionSkeletonProvider>(ProgrammingLanguage.VB.ToString());

            RegisterUnitTestProviders(container);
        }
    }
}
