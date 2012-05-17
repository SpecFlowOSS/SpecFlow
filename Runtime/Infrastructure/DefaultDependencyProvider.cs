using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal partial class DefaultDependencyProvider
    {
        partial void RegisterUnitTestProviders(ObjectContainer container);

        public virtual void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<DefaultRuntimeConfigurationProvider, IRuntimeConfigurationProvider>();

            container.RegisterTypeAs<TestRunnerFactory, ITestRunnerFactory>();
            container.RegisterTypeAs<TestRunner, ITestRunner>();
            container.RegisterTypeAs<TestExecutionEngine, ITestExecutionEngine>();
            container.RegisterTypeAs<StepDefinitionMatcher, IStepDefinitionMatcher>();

            container.RegisterTypeAs<StepFormatter, IStepFormatter>();
            container.RegisterTypeAs<TestTracer, ITestTracer>();

            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<ErrorProvider, IErrorProvider>();
            container.RegisterTypeAs<StepArgumentTypeConverter, IStepArgumentTypeConverter>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
            container.RegisterTypeAs<BindingFactory, IBindingFactory>();
            container.RegisterTypeAs<StepDefinitionRegexCalculator, IStepDefinitionRegexCalculator>();
            container.RegisterTypeAs<BindingInvoker, IBindingInvoker>();

            container.RegisterTypeAs<ContextManager, IContextManager>();

            container.RegisterTypeAs<StepDefinitionSkeletonProviderCS, IStepDefinitionSkeletonProvider>(ProgrammingLanguage.CSharp.ToString());
            container.RegisterTypeAs<StepDefinitionSkeletonProviderVB, IStepDefinitionSkeletonProvider>(ProgrammingLanguage.VB.ToString());

            container.RegisterTypeAs<RuntimePluginLoader, IRuntimePluginLoader>();

            RegisterUnitTestProviders(container);
        }
    }
}