using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
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
            container.RegisterTypeAs<StepDefinitionMatchService, IStepDefinitionMatchService>();

            container.RegisterTypeAs<StepFormatter, IStepFormatter>();
            container.RegisterTypeAs<TestTracer, ITestTracer>();

            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<ErrorProvider, IErrorProvider>();
            container.RegisterTypeAs<StepArgumentTypeConverter, IStepArgumentTypeConverter>();
            container.RegisterTypeAs<RuntimeBindingSourceProcessor, IRuntimeBindingSourceProcessor>();
            container.RegisterTypeAs<RuntimeBindingRegistryBuilder, IRuntimeBindingRegistryBuilder>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
            container.RegisterTypeAs<BindingFactory, IBindingFactory>();
            container.RegisterTypeAs<StepDefinitionRegexCalculator, IStepDefinitionRegexCalculator>();
            container.RegisterTypeAs<BindingInvoker, IBindingInvoker>();

            container.RegisterTypeAs<ContextManager, IContextManager>();

            container.RegisterTypeAs<StepDefinitionSkeletonProvider, IStepDefinitionSkeletonProvider>();
            container.RegisterTypeAs<DefaultSkeletonTemplateProvider, ISkeletonTemplateProvider>();
            container.RegisterTypeAs<StepTextAnalyzer, IStepTextAnalyzer>();

            container.RegisterTypeAs<RuntimePluginLoader, IRuntimePluginLoader>();

            container.RegisterTypeAs<BindingAssemblyLoader, IBindingAssemblyLoader>();

            RegisterUnitTestProviders(container);
        }
    }
}