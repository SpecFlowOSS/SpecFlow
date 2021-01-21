using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BoDi;
using Moq;
using Xunit;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using FluentAssertions;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{

    public class TestExecutionEngineTests
    {
        private ScenarioContext scenarioContext;
        private SpecFlowConfiguration specFlowConfiguration;
        private Mock<IBindingRegistry> bindingRegistryStub;
        private Mock<IErrorProvider> errorProviderStub;
        private Mock<IContextManager> contextManagerStub;
        private Mock<ITestTracer> testTracerStub;
        private Mock<IStepDefinitionMatchService> stepDefinitionMatcherStub;
        private Mock<IBindingInvoker> methodBindingInvokerMock;
        private Mock<IStepDefinitionSkeletonProvider> stepDefinitionSkeletonProviderMock;
        private Mock<ITestObjectResolver> testObjectResolverMock;
        private Mock<IObsoleteStepHandler> obsoleteTestHandlerMock;
        private Mock<ICucumberMessageSender> cucumberMessageSenderMock;
        private FeatureInfo featureInfo;
        private ScenarioInfo scenarioInfo;
        private ObjectContainer testThreadContainer;
        private ObjectContainer featureContainer;
        private ObjectContainer scenarioContainer;
        private TestObjectResolver defaultTestObjectResolver = new TestObjectResolver();
        private ITestPendingMessageFactory _testPendingMessageFactory;
        private ITestUndefinedMessageFactory _testUndefinedMessageFactory;
        private Mock<IAnalyticsEventProvider> _analyticsEventProvider;
        private Mock<IAnalyticsTransmitter> _analyticsTransmitter;
        private Mock<ITestRunnerManager> _testRunnerManager;
        private Mock<IRuntimePluginTestExecutionLifecycleEventEmitter> _runtimePluginTestExecutionLifecycleEventEmitter;

        private List<IHookBinding> beforeScenarioEvents;
        private List<IHookBinding> afterScenarioEvents;
        private List<IHookBinding> beforeStepEvents;
        private List<IHookBinding> afterStepEvents;
        private List<IHookBinding> beforeFeatureEvents;
        private List<IHookBinding> afterFeatureEvents;
        private List<IHookBinding> beforeTestRunEvents;
        private List<IHookBinding> afterTestRunEvents;
        private List<IHookBinding> beforeScenarioBlockEvents;
        private List<IHookBinding> afterScenarioBlockEvents;



        class DummyClass
        {
            public static DummyClass LastInstance = null;
            public DummyClass()
            {
                LastInstance = this;
            }
        }

        class AnotherDummyClass { }


        public TestExecutionEngineTests()
        {
            specFlowConfiguration = ConfigurationLoader.GetDefault();

            testThreadContainer = new ObjectContainer();
            featureContainer = new ObjectContainer();
            scenarioContainer = new ObjectContainer();

            beforeScenarioEvents = new List<IHookBinding>();
            afterScenarioEvents = new List<IHookBinding>();
            beforeStepEvents = new List<IHookBinding>();
            afterStepEvents = new List<IHookBinding>();
            beforeFeatureEvents = new List<IHookBinding>();
            afterFeatureEvents = new List<IHookBinding>();
            beforeTestRunEvents = new List<IHookBinding>();
            afterTestRunEvents = new List<IHookBinding>();
            beforeScenarioBlockEvents = new List<IHookBinding>();
            afterScenarioBlockEvents = new List<IHookBinding>();

            stepDefinitionSkeletonProviderMock = new Mock<IStepDefinitionSkeletonProvider>();
            testObjectResolverMock = new Mock<ITestObjectResolver>();
            testObjectResolverMock.Setup(bir => bir.ResolveBindingInstance(It.IsAny<Type>(), It.IsAny<IObjectContainer>()))
                .Returns((Type t, IObjectContainer container) => defaultTestObjectResolver.ResolveBindingInstance(t, container));

            var culture = new CultureInfo("en-US", false);
            contextManagerStub = new Mock<IContextManager>();
            scenarioInfo = new ScenarioInfo("scenario_title", "scenario_description", null, null);
            scenarioContext = new ScenarioContext(scenarioContainer, scenarioInfo, testObjectResolverMock.Object);
            scenarioContainer.RegisterInstanceAs(scenarioContext);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);
            featureInfo = new FeatureInfo(culture, "feature path", "feature_title", "", ProgrammingLanguage.CSharp);
            var featureContext = new FeatureContext(featureContainer, featureInfo, specFlowConfiguration);
            featureContainer.RegisterInstanceAs(featureContext);
            contextManagerStub.Setup(cm => cm.FeatureContext).Returns(featureContext);
            contextManagerStub.Setup(cm => cm.StepContext).Returns(new ScenarioStepContext(new StepInfo(StepDefinitionType.Given, "step_title", null, null)));

            bindingRegistryStub = new Mock<IBindingRegistry>();
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.BeforeStep)).Returns(beforeStepEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.AfterStep)).Returns(afterStepEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.BeforeScenarioBlock)).Returns(beforeScenarioBlockEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.AfterScenarioBlock)).Returns(afterScenarioBlockEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.BeforeFeature)).Returns(beforeFeatureEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.AfterFeature)).Returns(afterFeatureEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.BeforeTestRun)).Returns(beforeTestRunEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.AfterTestRun)).Returns(afterTestRunEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.BeforeScenario)).Returns(beforeScenarioEvents);
            bindingRegistryStub.Setup(br => br.GetHooks(HookType.AfterScenario)).Returns(afterScenarioEvents);

            specFlowConfiguration = ConfigurationLoader.GetDefault();
            errorProviderStub = new Mock<IErrorProvider>();
            testTracerStub = new Mock<ITestTracer>();
            stepDefinitionMatcherStub = new Mock<IStepDefinitionMatchService>();
            methodBindingInvokerMock = new Mock<IBindingInvoker>();

            obsoleteTestHandlerMock = new Mock<IObsoleteStepHandler>();

            cucumberMessageSenderMock = new Mock<ICucumberMessageSender>();
            cucumberMessageSenderMock.Setup(m => m.SendTestRunStarted())
                                     .Callback(() => { });

            _testPendingMessageFactory = new TestPendingMessageFactory(errorProviderStub.Object);
            _testUndefinedMessageFactory = new TestUndefinedMessageFactory(stepDefinitionSkeletonProviderMock.Object, errorProviderStub.Object, specFlowConfiguration);

            _analyticsEventProvider = new Mock<IAnalyticsEventProvider>();
            _analyticsTransmitter = new Mock<IAnalyticsTransmitter>();
            _analyticsTransmitter.Setup(at => at.TransmitSpecFlowProjectRunningEvent(It.IsAny<SpecFlowProjectRunningEvent>()))
                .Callback(() => { });

            _testRunnerManager = new Mock<ITestRunnerManager>();
            _testRunnerManager.Setup(trm => trm.TestAssembly).Returns(Assembly.GetCallingAssembly);

            _runtimePluginTestExecutionLifecycleEventEmitter = new Mock<IRuntimePluginTestExecutionLifecycleEventEmitter>();
        }

        private TestExecutionEngine CreateTestExecutionEngine()
        {
            return new TestExecutionEngine(
                new Mock<IStepFormatter>().Object,
                testTracerStub.Object,
                errorProviderStub.Object,
                new Mock<IStepArgumentTypeConverter>().Object,
                specFlowConfiguration,
                bindingRegistryStub.Object,
                new Mock<IUnitTestRuntimeProvider>().Object,
                contextManagerStub.Object,
                stepDefinitionMatcherStub.Object,
                methodBindingInvokerMock.Object,
                obsoleteTestHandlerMock.Object,
                cucumberMessageSenderMock.Object,
                new TestResultFactory(new TestResultPartsFactory(new TestErrorMessageFactory(), _testPendingMessageFactory, new TestAmbiguousMessageFactory(), _testUndefinedMessageFactory)),
                _testPendingMessageFactory,
                _testUndefinedMessageFactory,
                new Mock<ITestRunResultCollector>().Object,
                _analyticsEventProvider.Object,
                _analyticsTransmitter.Object,
                _testRunnerManager.Object,
                _runtimePluginTestExecutionLifecycleEventEmitter.Object,
                testObjectResolverMock.Object,
                testThreadContainer);
        }

        private Mock<IPickleIdStore> GetPickleIdStoreMock()
        {
            var dictionary = new Dictionary<ScenarioInfo, Guid>();
            var pickleIdStoreMock = new Mock<IPickleIdStore>();
            pickleIdStoreMock.Setup(m => m.GetPickleIdForScenario(It.IsAny<ScenarioInfo>()))
                             .Returns<ScenarioInfo>(info =>
                             {
                                 if (dictionary.ContainsKey(info))
                                 {
                                     return dictionary[info];
                                 }

                                 var newGuid = Guid.NewGuid();
                                 dictionary.Add(info, newGuid);
                                 return newGuid;
                             });
            return pickleIdStoreMock;
        }

        private Mock<IStepDefinitionBinding> RegisterStepDefinition()
        {
            var methodStub = new Mock<IBindingMethod>();
            var stepDefStub = new Mock<IStepDefinitionBinding>();
            stepDefStub.Setup(sd => sd.Method).Returns(methodStub.Object);

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            stepDefinitionMatcherStub.Setup(sdm => sdm.GetBestMatch(It.IsAny<StepInstance>(), It.IsAny<CultureInfo>(), out ambiguityReason, out candidatingMatches))
                .Returns(
                    new BindingMatch(stepDefStub.Object, 0, new object[0], new StepContext("bla", "foo", new List<string>(), CultureInfo.InvariantCulture)));

            return stepDefStub;
        }

        private Mock<IStepDefinitionBinding> RegisterUndefinedStepDefinition()
        {
            var methodStub = new Mock<IBindingMethod>();
            var stepDefStub = new Mock<IStepDefinitionBinding>();
            stepDefStub.Setup(sd => sd.Method).Returns(methodStub.Object);

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            stepDefinitionMatcherStub.Setup(sdm => sdm.GetBestMatch(It.IsAny<StepInstance>(), It.IsAny<CultureInfo>(), out ambiguityReason, out candidatingMatches))
                .Returns(BindingMatch.NonMatching);

            return stepDefStub;
        }

        private void RegisterFailingStepDefinition(TimeSpan? expectedDuration = null)
        {
            var stepDefStub = RegisterStepDefinition();
            TimeSpan duration;
            if (expectedDuration.HasValue) duration = expectedDuration.Value;
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(stepDefStub.Object, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, out duration))
                .Throws(new Exception("simulated error"));
        }

        private Mock<IHookBinding> CreateHookMock(List<IHookBinding> hookList)
        {
            var mock = new Mock<IHookBinding>();
            hookList.Add(mock.Object);
            return mock;
        }

        private Mock<IHookBinding> CreateParametrizedHookMock(List<IHookBinding> hookList, params Type[] paramTypes)
        {
            var hookMock = CreateHookMock(hookList);
            var bindingMethod = new BindingMethod(new BindingType("AssemblyBT", "BT", "Test.BT"), "X",
                paramTypes.Select((paramType, i) => new BindingParameter(new RuntimeBindingType(paramType), "p" + i)),
                RuntimeBindingType.Void);
            hookMock.Setup(h => h.Method).Returns(bindingMethod);
            return hookMock;
        }

        private void AssertHooksWasCalledWithParam(Mock<IHookBinding> hookMock, object paramObj)
        {
            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object,
                It.Is((object[] args) => args != null && args.Length > 0 && args.Any(arg => arg == paramObj)),
                testTracerStub.Object, out duration), Times.Once());
        }

        [Fact]
        public void Should_execute_before_step()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(beforeStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration), Times.Once());
        }

        [Fact]
        public void Should_execute_after_step()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration), Times.Once());
        }

        [Fact]
        public void Should_not_execute_step_when_there_was_an_error_earlier()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            var stepDefMock = RegisterStepDefinition();

            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(stepDefMock.Object, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration), Times.Never());
        }

        [Fact]
        public void Should_not_execute_step_hooks_when_there_was_an_error_earlier()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            var beforeStepMock = CreateHookMock(beforeStepEvents);
            var afterStepMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(beforeStepMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration), Times.Never());
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(afterStepMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration), Times.Never());
        }

        [Fact]
        public void Should_execute_after_step_when_step_definition_failed()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterFailingStepDefinition();

            var hookMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration));
        }

        [Fact]
        public void Should_cleanup_step_context_after_scenario_block_hook_error()
        {
            TimeSpan duration;
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(beforeScenarioBlockEvents);
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration))
                .Throws(new Exception("simulated error"));

            try
            {
                testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

                Assert.True(false, "execution of the step should have failed because of the exeption thrown by the before scenario block hook");
            }
            catch (Exception)
            {
            }

            methodBindingInvokerMock.Verify(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration), Times.Once());
            contextManagerStub.Verify(cm => cm.CleanupStepContext());
        }

        [Fact]
        public void Should_not_execute_afterstep_when_step_is_undefined()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterUndefinedStepDefinition();

            var afterStepMock = CreateHookMock(afterStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "undefined", null, null);

            TimeSpan duration;
            methodBindingInvokerMock.Verify(i => i.InvokeBinding(afterStepMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration), Times.Never());
        }

        [Fact]
        public void Should_resolve_FeatureContext_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeFeatureEvents, typeof(FeatureContext));

            testExecutionEngine.OnFeatureStart(featureInfo);
            AssertHooksWasCalledWithParam(hookMock, contextManagerStub.Object.FeatureContext);
        }

        [Fact]
        public void Should_resolve_custom_class_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeFeatureEvents, typeof(DummyClass));

            testExecutionEngine.OnFeatureStart(featureInfo);
            AssertHooksWasCalledWithParam(hookMock, DummyClass.LastInstance);
        }

        [Fact]
        public void Should_resolve_container_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeTestRunEvents, typeof(IObjectContainer));

            testExecutionEngine.OnTestRunStart();

            AssertHooksWasCalledWithParam(hookMock, testThreadContainer);
        }

        [Fact]
        public void Should_resolve_multiple_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeFeatureEvents, typeof(DummyClass), typeof(FeatureContext));

            testExecutionEngine.OnFeatureStart(featureInfo);
            AssertHooksWasCalledWithParam(hookMock, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(hookMock, contextManagerStub.Object.FeatureContext);
        }

        [Fact]
        public void Should_resolve_BeforeAfterTestRun_hook_parameter_from_test_thread_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeTestRunEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterTestRunEvents, typeof(DummyClass));

            testExecutionEngine.OnTestRunStart();
            testExecutionEngine.OnTestRunEnd();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), testThreadContainer),
                Times.Exactly(2));
        }

        [Fact]
        public void Should_resolve_BeforeAfterScenario_hook_parameter_from_scenario_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeScenarioEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterScenarioEvents, typeof(DummyClass));

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioStart();
            testExecutionEngine.OnScenarioEnd();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), scenarioContainer),
                Times.Exactly(2));
        }

        [Fact]
        public void Should_be_possible_to_register_instance_in_scenario_container_before_firing_scenario_events()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            var instanceToAddBeforeScenarioEventFiring = new AnotherDummyClass();
            var beforeHook = CreateParametrizedHookMock(beforeScenarioEvents, typeof(DummyClass));

            // Setup binding method mock so it attempts to resolve an instance from the scenario container.
            // If this fails, then the instance was not registered before the method was invoked.
            TimeSpan dummyOutTimeSpan;
            AnotherDummyClass actualInstance = null;
            methodBindingInvokerMock.Setup(s => s.InvokeBinding(It.IsAny<IBinding>(), It.IsAny<IContextManager>(),
                    It.IsAny<object[]>(),It.IsAny<ITestTracer>(), out dummyOutTimeSpan))
                .Callback(() => actualInstance = testExecutionEngine.ScenarioContext.ScenarioContainer.Resolve<AnotherDummyClass>());

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.ScenarioContext.ScenarioContainer.RegisterInstanceAs(instanceToAddBeforeScenarioEventFiring);
            testExecutionEngine.OnScenarioStart();
            actualInstance.Should().BeSameAs(instanceToAddBeforeScenarioEventFiring);

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
        }

        [Fact]
        public void Should_resolve_BeforeAfterScenarioBlock_hook_parameter_from_scenario_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeScenarioBlockEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterScenarioBlockEvents, typeof(DummyClass));

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            testExecutionEngine.OnAfterLastStep();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), scenarioContainer),
                Times.Exactly(2));
        }

        [Fact]
        public void Should_resolve_BeforeAfterStep_hook_parameter_from_scenario_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeStepEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterStepEvents, typeof(DummyClass));

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), scenarioContainer),
                Times.Exactly(2));
        }

        [Fact]
        public void Should_resolve_BeforeAfterFeature_hook_parameter_from_feature_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeFeatureEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterFeatureEvents, typeof(DummyClass));

            testExecutionEngine.OnFeatureStart(featureInfo);
            testExecutionEngine.OnFeatureEnd();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), featureContainer),
                Times.Exactly(2));
        }

        [Fact]
        public void Should_TryToSend_ProjectRunningEvent()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnTestRunStart();

            _analyticsTransmitter.Verify(at => at.TransmitSpecFlowProjectRunningEvent(It.IsAny<SpecFlowProjectRunningEvent>()), Times.Once);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(3, 1)]
        public void Should_execute_all_ISkippedStepHandlers_for_each_skipped_step(int numberOfHandlers, int numberOfSkippedSteps)
        {
            var sut = CreateTestExecutionEngine();
            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            var skippedStepHandlerMocks = new List<Mock<ISkippedStepHandler>>();
            for (int i = 0; i < numberOfHandlers; i++)
            {
                var mockHandler = new Mock<ISkippedStepHandler>();
                mockHandler.Setup(b => b.Handle(It.IsAny<ScenarioContext>())).Verifiable();
                skippedStepHandlerMocks.Add(mockHandler);
                scenarioContext.ScenarioContainer.RegisterInstanceAs(mockHandler.Object, i.ToString());
            }

            for (int i = 0; i < numberOfSkippedSteps; i++)
            {
                RegisterStepDefinition();
                sut.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            }

            foreach (var handler in skippedStepHandlerMocks)
            {
                handler.Verify(action => action.Handle(It.IsAny<ScenarioContext>()), Times.Exactly(numberOfSkippedSteps));
            }
        }

        [Fact]
        public void Should_not_change_ScenarioExecutionStatus_on_dummy_ISkippedStepHandler()
        {
            var sut = CreateTestExecutionEngine();
            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            var mockHandler = new Mock<ISkippedStepHandler>();
            mockHandler.Setup(b => b.Handle(It.IsAny<ScenarioContext>())).Callback(() => Console.WriteLine("ISkippedStepHandler"));
            scenarioContext.ScenarioContainer.RegisterInstanceAs(mockHandler.Object);

            RegisterStepDefinition();
            sut.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            scenarioContext.ScenarioExecutionStatus.Should().Be(ScenarioExecutionStatus.TestError);
        }

        [Fact]
        public void Should_not_call_ISkippedStepHandler_on_UndefinedStepDefinition()
        {
            var sut = CreateTestExecutionEngine();
            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            var mockHandler = new Mock<ISkippedStepHandler>();
            mockHandler.Setup(b => b.Handle(It.IsAny<ScenarioContext>())).Verifiable();
            scenarioContext.ScenarioContainer.RegisterInstanceAs(mockHandler.Object);

            RegisterUndefinedStepDefinition();
            sut.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            mockHandler.Verify(action => action.Handle(It.IsAny<ScenarioContext>()), Times.Never);
        }

        [Fact]
        public void Should_not_call_ISkippedStepHandler_on_succesfull_test_run()
        {
            var sut = CreateTestExecutionEngine();

            var mockHandler = new Mock<ISkippedStepHandler>();
            mockHandler.Setup(b => b.Handle(It.IsAny<ScenarioContext>())).Verifiable();
            scenarioContext.ScenarioContainer.RegisterInstanceAs(mockHandler.Object);

            RegisterStepDefinition();
            sut.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            mockHandler.Verify(action => action.Handle(It.IsAny<ScenarioContext>()), Times.Never);
        }

        [Fact]
        public void Should_not_call_ISkippedStepHandler_if_only_last_step_is_failing()
        {
            var sut = CreateTestExecutionEngine();

            var mockHandler = new Mock<ISkippedStepHandler>();
            mockHandler.Setup(b => b.Handle(It.IsAny<ScenarioContext>())).Callback(() => Console.WriteLine("ISkippedStepHandler"));
            scenarioContext.ScenarioContainer.RegisterInstanceAs(mockHandler.Object);

            RegisterFailingStepDefinition();
            sut.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            mockHandler.Verify(action => action.Handle(It.IsAny<ScenarioContext>()), Times.Never);
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforetestrun()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnTestRunStart();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.BeforeTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_aftertestrun()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnTestRunEnd();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.AfterTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforefeature()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnFeatureStart(featureInfo);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.BeforeFeature, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_afterfeature()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnFeatureEnd();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.AfterFeature, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforescenario()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioStart();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.BeforeScenario, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_afterscenario()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioEnd();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.AfterScenario, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforestep()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.BeforeStep, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_afterstep()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.AfterStep, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_set_correct_duration_in_case_of_failed_step()
        {
            TimeSpan executionDuration = TimeSpan.Zero;
            testTracerStub.Setup(c => c.TraceError(It.IsAny<Exception>(), It.IsAny<TimeSpan>()))
                          .Callback<Exception, TimeSpan>((ex, duration) => executionDuration = duration);

            var testExecutionEngine = CreateTestExecutionEngine();

            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
            RegisterFailingStepDefinition(expectedDuration);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            testTracerStub.Verify(tracer => tracer.TraceError(It.IsAny<Exception>(), It.IsAny<TimeSpan>()), Times.Once());
            executionDuration.Should().Be(expectedDuration);
        }

        [Fact]
        public void Should_set_correct_duration_in_case_of_passed_step()
        {
            TimeSpan executionDuration = TimeSpan.Zero;
            testTracerStub.Setup(c => c.TraceStepDone(It.IsAny<BindingMatch>(), It.IsAny<object[]>(), It.IsAny<TimeSpan>()))
                          .Callback<BindingMatch, object[], TimeSpan>((match, arguments, duration) => executionDuration = duration);

            var testExecutionEngine = CreateTestExecutionEngine();

            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
            var stepDefStub = RegisterStepDefinition();
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(stepDefStub.Object, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, out expectedDuration));

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            testTracerStub.Verify(tracer => tracer.TraceStepDone(It.IsAny<BindingMatch>(), It.IsAny<object[]>(), It.IsAny<TimeSpan>()), Times.Once());
            executionDuration.Should().Be(expectedDuration);
        }

    }
}