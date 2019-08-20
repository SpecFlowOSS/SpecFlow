using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
using TechTalk.SpecFlow.CucumberMessages;

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
        private Dictionary<string, IStepErrorHandler> stepErrorHandlers;
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

            var culture = new CultureInfo("en-US");
            contextManagerStub = new Mock<IContextManager>();
            scenarioInfo = new ScenarioInfo("scenario_title", "scenario_description");
            scenarioContext = new ScenarioContext(scenarioContainer, scenarioInfo, testObjectResolverMock.Object);
            scenarioContainer.RegisterInstanceAs(scenarioContext);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);
            featureInfo = new FeatureInfo(culture, "feature_title", "", ProgrammingLanguage.CSharp);
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

            stepErrorHandlers = new Dictionary<string, IStepErrorHandler>();
            obsoleteTestHandlerMock = new Mock<IObsoleteStepHandler>();

            cucumberMessageSenderMock = new Mock<ICucumberMessageSender>();
            cucumberMessageSenderMock.Setup(m => m.SendTestRunStarted())
                                     .Callback(() => { });

            _testPendingMessageFactory = new TestPendingMessageFactory(errorProviderStub.Object);
            _testUndefinedMessageFactory = new TestUndefinedMessageFactory(stepDefinitionSkeletonProviderMock.Object, errorProviderStub.Object, specFlowConfiguration);
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
                stepErrorHandlers, 
                methodBindingInvokerMock.Object,
                obsoleteTestHandlerMock.Object,
                cucumberMessageSenderMock.Object,
                new TestResultFactory(new TestResultPartsFactory(new TestErrorMessageFactory(), _testPendingMessageFactory, new TestAmbiguousMessageFactory(), _testUndefinedMessageFactory)),
                _testPendingMessageFactory,
                _testUndefinedMessageFactory,
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

        private void RegisterFailingStepDefinition()
        {
            var stepDefStub = RegisterStepDefinition();
            methodBindingInvokerMock.Setup(i => i.InvokeBindingAsync(stepDefStub.Object, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object))
                .ThrowsAsync(new Exception("simulated error"));
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
            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object,
                It.Is((object[] args) => args != null && args.Length > 0 && args.Any(arg => arg == paramObj)),
                testTracerStub.Object), Times.Once());
        }

        [Fact]
        public async Task Should_execute_before_step()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(beforeStepEvents);

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object), Times.Once());
        }

        [Fact]
        public async Task Should_execute_after_step()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(afterStepEvents);

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object), Times.Once());
        }

        [Fact]
        public async Task Should_not_execute_step_when_there_was_an_error_earlier()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            var stepDefMock = RegisterStepDefinition();

            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(stepDefMock.Object, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>()), Times.Never());
        }

        [Fact]
        public async Task Should_not_execute_step_hooks_when_there_was_an_error_earlier()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            var beforeStepMock = CreateHookMock(beforeStepEvents);
            var afterStepMock = CreateHookMock(afterStepEvents);

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(beforeStepMock.Object, contextManagerStub.Object, null, testTracerStub.Object), Times.Never());
            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(afterStepMock.Object, contextManagerStub.Object, null, testTracerStub.Object), Times.Never());
        }

        [Fact]
        public async Task Should_execute_after_step_when_step_definition_failed()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterFailingStepDefinition();

            var hookMock = CreateHookMock(afterStepEvents);

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object));
        }

        [Fact]
        public async Task Should_call_step_error_handlers()
        {
            var stepErrorHandlerMock = new Mock<IStepErrorHandler>();
            stepErrorHandlers.Add("eh1", stepErrorHandlerMock.Object);

            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterFailingStepDefinition();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            stepErrorHandlerMock.Verify(seh => seh.OnStepFailure(testExecutionEngine, It.IsAny<StepFailureEventArgs>()), Times.Once());
        }


        [Fact]
        public async Task Should_call_multiple_step_error_handlers()
        {
            var stepErrorHandler1Mock = new Mock<IStepErrorHandler>();
            var stepErrorHandler2Mock = new Mock<IStepErrorHandler>();
            stepErrorHandlers.Add("eh1", stepErrorHandler1Mock.Object);
            stepErrorHandlers.Add("eh2", stepErrorHandler2Mock.Object);

            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterFailingStepDefinition();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            stepErrorHandler1Mock.Verify(seh => seh.OnStepFailure(testExecutionEngine, It.IsAny<StepFailureEventArgs>()), Times.Once());
            stepErrorHandler2Mock.Verify(seh => seh.OnStepFailure(testExecutionEngine, It.IsAny<StepFailureEventArgs>()), Times.Once());
        }

        [Fact]
        public async Task Should_be_able_to_swallow_error_in_step_error_handlers()
        {
            var stepErrorHandlerStub = new Mock<IStepErrorHandler>();
            stepErrorHandlers.Add("eh1", stepErrorHandlerStub.Object);

            stepErrorHandlerStub.Setup(seh => seh.OnStepFailure(It.IsAny<TestExecutionEngine>(), It.IsAny<StepFailureEventArgs>()))
                .Callback((TestExecutionEngine _, StepFailureEventArgs args) => args.IsHandled = true);

            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterFailingStepDefinition();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            scenarioContext.ScenarioExecutionStatus.Should().Be(ScenarioExecutionStatus.OK);
        }

        [Fact]
        public async Task Step_error_handlers_should_not_swallow_error_by_default()
        {
            var stepErrorHandlerStub = new Mock<IStepErrorHandler>();
            stepErrorHandlers.Add("eh1", stepErrorHandlerStub.Object);

            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterFailingStepDefinition();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            scenarioContext.ScenarioExecutionStatus.Should().Be(ScenarioExecutionStatus.TestError);
        }

        [Fact]
        public async Task Should_cleanup_step_context_after_scenario_block_hook_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(beforeScenarioBlockEvents);
            methodBindingInvokerMock.Setup(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object))
                .ThrowsAsync(new Exception("simulated error"));

            try
            {
                await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

                Assert.True(false, "execution of the step should have failed because of the exeption thrown by the before scenario block hook");
            }
            catch (Exception)
            {
            }

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object), Times.Once());
            contextManagerStub.Verify(cm => cm.CleanupStepContext());
        }

        [Fact]
        public async Task Should_not_execute_afterstep_when_step_is_undefined()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterUndefinedStepDefinition();

            var afterStepMock = CreateHookMock(afterStepEvents);

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "undefined", null, null);

            methodBindingInvokerMock.Verify(i => i.InvokeBindingAsync(afterStepMock.Object, contextManagerStub.Object, null, testTracerStub.Object), Times.Never());
        }

        public async Task Should_resolve_FeautreContext_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeFeatureEvents, typeof(FeatureContext));

            await testExecutionEngine.OnFeatureStartAsync(featureInfo);
            AssertHooksWasCalledWithParam(hookMock, contextManagerStub.Object.FeatureContext);
        }

        [Fact]
        public async Task Should_resolve_custom_class_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeFeatureEvents, typeof(DummyClass));

            await testExecutionEngine.OnFeatureStartAsync(featureInfo);
            AssertHooksWasCalledWithParam(hookMock, DummyClass.LastInstance);
        }

        [Fact]
        public async Task Should_resolve_container_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeTestRunEvents, typeof(IObjectContainer));

            await testExecutionEngine.OnTestRunStartAsync();

            AssertHooksWasCalledWithParam(hookMock, testThreadContainer);
        }

        [Fact]
        public async Task Should_resolve_multiple_hook_parameter()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateParametrizedHookMock(beforeFeatureEvents, typeof(DummyClass), typeof(FeatureContext));

            await testExecutionEngine.OnFeatureStartAsync(featureInfo);
            AssertHooksWasCalledWithParam(hookMock, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(hookMock, contextManagerStub.Object.FeatureContext);
        }

        [Fact]
        public async Task Should_resolve_BeforeAfterTestRun_hook_parameter_from_test_thread_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeTestRunEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterTestRunEvents, typeof(DummyClass));

            await testExecutionEngine.OnTestRunStartAsync();
            await testExecutionEngine.OnTestRunEndAsync();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), testThreadContainer), 
                Times.Exactly(2));
        }

        [Fact]
        public async Task Should_resolve_BeforeAfterScenario_hook_parameter_from_scenario_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeScenarioEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterScenarioEvents, typeof(DummyClass));

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            await testExecutionEngine.OnScenarioStartAsync();
            await testExecutionEngine.OnScenarioEndAsync();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), scenarioContainer),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Should_be_possible_to_register_instance_in_scenario_container_before_firing_scenario_events()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            var instanceToAddBeforeScenarioEventFiring = new AnotherDummyClass();
            var beforeHook = CreateParametrizedHookMock(beforeScenarioEvents, typeof(DummyClass));

            // Setup binding method mock so it attempts to resolve an instance from the scenario container.
            // If this fails, then the instance was not registered before the method was invoked.
            AnotherDummyClass actualInstance = null;
            methodBindingInvokerMock.Setup(s => s.InvokeBindingAsync(It.IsAny<IBinding>(), It.IsAny<IContextManager>(), 
                    It.IsAny<object[]>(),It.IsAny<ITestTracer>()))
                .Callback(() => actualInstance = testExecutionEngine.ScenarioContext.ScenarioContainer.Resolve<AnotherDummyClass>())
                .ReturnsAsync((null, new TimeSpan()));

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.ScenarioContext.ScenarioContainer.RegisterInstanceAs(instanceToAddBeforeScenarioEventFiring);
            await testExecutionEngine.OnScenarioStartAsync();
            actualInstance.Should().BeSameAs(instanceToAddBeforeScenarioEventFiring);

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
        }

        [Fact]
        public async Task Should_resolve_BeforeAfterScenarioBlock_hook_parameter_from_scenario_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeScenarioBlockEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterScenarioBlockEvents, typeof(DummyClass));

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
            await testExecutionEngine.OnAfterLastStepAsync();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), scenarioContainer),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Should_resolve_BeforeAfterStep_hook_parameter_from_scenario_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeStepEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterStepEvents, typeof(DummyClass));

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), scenarioContainer),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Should_resolve_BeforeAfterFeature_hook_parameter_from_feature_container()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var beforeHook = CreateParametrizedHookMock(beforeFeatureEvents, typeof(DummyClass));
            var afterHook = CreateParametrizedHookMock(afterFeatureEvents, typeof(DummyClass));

            await testExecutionEngine.OnFeatureStartAsync(featureInfo);
            await testExecutionEngine.OnFeatureEndAsync();

            AssertHooksWasCalledWithParam(beforeHook, DummyClass.LastInstance);
            AssertHooksWasCalledWithParam(afterHook, DummyClass.LastInstance);
            testObjectResolverMock.Verify(bir => bir.ResolveBindingInstance(typeof(DummyClass), featureContainer),
                Times.Exactly(2));
        }
    }
}
