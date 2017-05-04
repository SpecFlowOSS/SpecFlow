using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    //NOTE: Please update https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations if you change registration defaults

    public partial class DefaultDependencyProvider : IDefaultDependencyProvider
    {
        partial void RegisterUnitTestProviders(ObjectContainer container);

        public virtual void RegisterGlobalContainerDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<DefaultRuntimeConfigurationProvider, IRuntimeConfigurationProvider>();

            container.RegisterTypeAs<TestRunnerManager, ITestRunnerManager>();

            container.RegisterTypeAs<StepFormatter, IStepFormatter>();
            container.RegisterTypeAs<TestTracer, ITestTracer>();

            container.RegisterTypeAs<DefaultListener, ITraceListener>();
            container.RegisterTypeAs<TraceListenerQueue, ITraceListenerQueue>(); 

            container.RegisterTypeAs<ErrorProvider, IErrorProvider>();
            container.RegisterTypeAs<RuntimeBindingSourceProcessor, IRuntimeBindingSourceProcessor>();
            container.RegisterTypeAs<RuntimeBindingRegistryBuilder, IRuntimeBindingRegistryBuilder>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
            container.RegisterTypeAs<BindingFactory, IBindingFactory>();
            container.RegisterTypeAs<StepDefinitionRegexCalculator, IStepDefinitionRegexCalculator>();
            container.RegisterTypeAs<BindingInvoker, IBindingInvoker>();
            container.RegisterTypeAs<TestObjectResolver, ITestObjectResolver>();

            container.RegisterTypeAs<StepDefinitionSkeletonProvider, IStepDefinitionSkeletonProvider>();
            container.RegisterTypeAs<DefaultSkeletonTemplateProvider, ISkeletonTemplateProvider>();
            container.RegisterTypeAs<StepTextAnalyzer, IStepTextAnalyzer>();

            container.RegisterTypeAs<RuntimePluginLoader, IRuntimePluginLoader>();

            container.RegisterTypeAs<BindingAssemblyLoader, IBindingAssemblyLoader>();

            container.RegisterTypeAs<ConfigurationLoader, IConfigurationLoader>();

            RegisterUnitTestProviders(container);
        }

        public void RegisterTestThreadContainerDefaults(ObjectContainer testThreadContainer)
        {
            testThreadContainer.RegisterTypeAs<TestRunner, ITestRunner>();
            testThreadContainer.RegisterTypeAs<ContextManager, IContextManager>();
            testThreadContainer.RegisterTypeAs<TestExecutionEngine, ITestExecutionEngine>();

            // needs to invoke methods so requires the context manager
            testThreadContainer.RegisterTypeAs<StepArgumentTypeConverter, IStepArgumentTypeConverter>();
            testThreadContainer.RegisterTypeAs<StepDefinitionMatchService, IStepDefinitionMatchService>();

            testThreadContainer.RegisterTypeAs<AsyncTraceListener, ITraceListener>();
            testThreadContainer.RegisterTypeAs<TestTracer, ITestTracer>();
        }
    }
}