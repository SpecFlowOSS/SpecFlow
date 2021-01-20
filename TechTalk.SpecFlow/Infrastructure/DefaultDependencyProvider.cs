using BoDi;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Analytics.AppInsights;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using TechTalk.SpecFlow.EnvironmentAccess;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.FileAccess;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.TestFramework;
using TechTalk.SpecFlow.Time;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    //NOTE: Please update https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations if you change registration defaults

    public class DefaultDependencyProvider : IDefaultDependencyProvider
    {
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
            container.RegisterTypeAs<SpecFlowAttributesFilter, ISpecFlowAttributesFilter>();
            container.RegisterTypeAs<BindingRegistry, IBindingRegistry>();
            container.RegisterTypeAs<BindingFactory, IBindingFactory>();
            container.RegisterTypeAs<StepDefinitionRegexCalculator, IStepDefinitionRegexCalculator>();
            container.RegisterTypeAs<BindingInvoker, IBindingInvoker>();
            container.RegisterTypeAs<SynchronousBindingDelegateInvoker, ISynchronousBindingDelegateInvoker>();
            container.RegisterTypeAs<TestObjectResolver, ITestObjectResolver>();

            container.RegisterTypeAs<StepDefinitionSkeletonProvider, IStepDefinitionSkeletonProvider>();
            container.RegisterTypeAs<DefaultSkeletonTemplateProvider, ISkeletonTemplateProvider>();
            container.RegisterTypeAs<StepTextAnalyzer, IStepTextAnalyzer>();

            container.RegisterTypeAs<RuntimePluginLoader, IRuntimePluginLoader>();
            container.RegisterTypeAs<RuntimePluginLocator, IRuntimePluginLocator>();
            container.RegisterTypeAs<RuntimePluginLocationMerger, IRuntimePluginLocationMerger>();

            container.RegisterTypeAs<BindingAssemblyLoader, IBindingAssemblyLoader>();

            container.RegisterTypeAs<ConfigurationLoader, IConfigurationLoader>();

            container.RegisterTypeAs<ObsoleteStepHandler, IObsoleteStepHandler>();

            container.RegisterTypeAs<EnvironmentWrapper, IEnvironmentWrapper>();
            container.RegisterTypeAs<BinaryFileAccessor, IBinaryFileAccessor>();
            container.RegisterTypeAs<TestErrorMessageFactory, ITestErrorMessageFactory>();
            container.RegisterTypeAs<TestPendingMessageFactory, ITestPendingMessageFactory>();
            container.RegisterTypeAs<TestAmbiguousMessageFactory, ITestAmbiguousMessageFactory>();
            container.RegisterTypeAs<TestUndefinedMessageFactory, ITestUndefinedMessageFactory>();
            container.RegisterTypeAs<ProtobufFileSinkOutput, IProtobufFileSinkOutput>();
            container.RegisterTypeAs<ProtobufFileNameResolver, IProtobufFileNameResolver>();
            container.RegisterTypeAs<SinkProvider, ISinkProvider>();
            container.RegisterTypeAs<DefaultTestRunContext, ITestRunContext>();

            container.RegisterTypeAs<SpecFlowPath, ISpecFlowPath>();

            container.RegisterTypeAs<UtcDateTimeClock, IClock>();
            container.RegisterTypeAs<CucumberMessageFactory, ICucumberMessageFactory>();
            container.RegisterTypeAs<TestResultFactory, ITestResultFactory>();
            container.RegisterTypeAs<TestResultPartsFactory, ITestResultPartsFactory>();
            container.RegisterTypeAs<PlatformFactory, IPlatformFactory>();
            container.RegisterTypeAs<SystemInformationProvider, ISystemInformationProvider>();
            container.RegisterTypeAs<SpecFlowVersionInformationProvider, ISpecFlowVersionInformationProvider>();
            container.RegisterTypeAs<CucumberMessageSender, ICucumberMessageSender>();
            container.RegisterTypeAs<PickleIdGenerator, IPickleIdGenerator>();
            container.RegisterTypeAs<PickleIdStore, IPickleIdStore>();
            container.RegisterTypeAs<PickleIdStoreDictionaryFactory, IPickleIdStoreDictionaryFactory>();
            container.RegisterTypeAs<FieldValueProvider, IFieldValueProvider>();

            container.RegisterTypeAs<NonStrictTestRunResultSuccessCalculator, ITestRunResultSuccessCalculator>();
            container.RegisterTypeAs<TestRunResultCollector, ITestRunResultCollector>();

            container.RegisterTypeAs<FileUserIdStore, IUserUniqueIdStore>();
            container.RegisterTypeAs<FileService, IFileService>();
            container.RegisterTypeAs<DirectoryService, IDirectoryService>();

            container.RegisterTypeAs<EnvironmentSpecFlowTelemetryChecker, IEnvironmentSpecFlowTelemetryChecker>();
            container.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            container.RegisterTypeAs<HttpClientAnalyticsTransmitterSink, IAnalyticsTransmitterSink>();
            container.RegisterTypeAs<AppInsightsEventSerializer, IAppInsightsEventSerializer>();
            container.RegisterTypeAs<HttpClientWrapper, HttpClientWrapper>();
            container.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();
            
            container.RegisterTypeAs<SpecFlowJsonLocator, ISpecFlowJsonLocator>();

            container.RegisterTypeAs<RuntimePluginTestExecutionLifecycleEvents, RuntimePluginTestExecutionLifecycleEvents>();
            container.RegisterTypeAs<RuntimePluginTestExecutionLifecycleEventEmitter, IRuntimePluginTestExecutionLifecycleEventEmitter>();

            container.RegisterTypeAs<TestAssemblyProvider, ITestAssemblyProvider>();
        }

        public virtual void RegisterTestThreadContainerDefaults(ObjectContainer testThreadContainer)
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

        public void RegisterScenarioContainerDefaults(ObjectContainer scenarioContainer)
        {
            scenarioContainer.RegisterTypeAs<SpecFlowOutputHelper, ISpecFlowOutputHelper>();
            scenarioContainer.RegisterTypeAs<SpecFlowScenarioOutputTracer, ISpecFlowScenarioOutputListener>("tracer");
        }
    }
}