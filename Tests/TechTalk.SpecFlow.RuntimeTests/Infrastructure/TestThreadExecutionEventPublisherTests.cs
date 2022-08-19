using System;
using System.Threading.Tasks;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Events;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public partial class TestExecutionEngineTests
    {
        [Fact]
        public async Task Should_publish_step_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                    te.PublishEvent(It.Is<StepStartedEvent>(e => e.ScenarioContext.Equals(scenarioContext) &&
                                                                 e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()) &&
                                                                 e.StepContext.Equals(contextManagerStub.Object.StepContext))), 
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_step_binding_started_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<StepBindingStartedEvent>(e => 
                                                                   e.StepDefinitionBinding.Equals(stepDef))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_step_binding_finished_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
            methodBindingInvokerMock
                .Setup(i => i.InvokeBindingAsync(stepDef, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, It.IsAny<DurationHolder>()))
                .Callback((IBinding _, IContextManager _, object[] arguments, ITestTracer _, DurationHolder durationHolder) => durationHolder.Duration = expectedDuration)
                .ReturnsAsync(new object());
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<StepBindingFinishedEvent>(e =>
                                                                    e.StepDefinitionBinding.Equals(stepDef) && 
                                                                    e.Duration.Equals(expectedDuration))),
                                                      Times.Once);
        }
        
        [Fact]
        public async Task Should_publish_step_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<StepFinishedEvent>(e => 
                                                             e.ScenarioContext.Equals(scenarioContext) &&
                                                             e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()) &&
                                                             e.StepContext.Equals(contextManagerStub.Object.StepContext))),
                                                      Times.Once);
        }
        
        [Fact]
        public async Task Should_publish_step_skipped_event()
        {
            RegisterStepDefinition();
            var testExecutionEngine = CreateTestExecutionEngine();
            //a step will be skipped if the ScenarioExecutionStatus is not OK
            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepSkippedEvent>()), Times.Once);
        }
        
        [Fact]
        public async Task Should_publish_hook_binding_events()
        {
            var hookType = HookType.AfterScenario;
            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
            var expectedHookBinding = new HookBinding(new Mock<IBindingMethod>().Object, hookType, null, 1);
            methodBindingInvokerMock
                .Setup(i => i.InvokeBindingAsync(expectedHookBinding, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, It.IsAny<DurationHolder>()))
                .Callback((IBinding _, IContextManager _, object[] arguments, ITestTracer _, DurationHolder durationHolder) => durationHolder.Duration = expectedDuration)
                .ReturnsAsync(new object());
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.InvokeHookAsync(methodBindingInvokerMock.Object, expectedHookBinding, hookType);

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<HookBindingStartedEvent>(e =>
                                                                   e.HookBinding.Equals(expectedHookBinding))),
                                                      Times.Once);
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<HookBindingFinishedEvent>(e =>
                                                                    e.HookBinding.Equals(expectedHookBinding) &&
                                                                    e.Duration.Equals(expectedDuration))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_scenario_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            await testExecutionEngine.OnScenarioStartAsync();

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<ScenarioStartedEvent>(e =>
                                                                e.ScenarioContext.Equals(scenarioContext) &&
                                                                e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_scenario_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            await testExecutionEngine.OnScenarioStartAsync();
            await testExecutionEngine.OnAfterLastStepAsync();
            await testExecutionEngine.OnScenarioEndAsync();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<ScenarioFinishedEvent>(e =>
                                                                 e.ScenarioContext.Equals(scenarioContext) &&
                                                                 e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_hook_started_finished_events()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            await testExecutionEngine.OnTestRunStartAsync();
            await testExecutionEngine.OnFeatureStartAsync(featureInfo);

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            await testExecutionEngine.OnScenarioStartAsync();
            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
            await testExecutionEngine.OnAfterLastStepAsync();
            await testExecutionEngine.OnScenarioEndAsync();

            await testExecutionEngine.OnFeatureEndAsync();
            await testExecutionEngine.OnTestRunEndAsync();

            AssertHookEventsForHookType(HookType.BeforeTestRun);
            AssertHookEventsForHookType(HookType.AfterTestRun);
            AssertHookEventsForHookType(HookType.BeforeFeature);
            AssertHookEventsForHookType(HookType.AfterFeature);
            AssertHookEventsForHookType(HookType.BeforeScenario);
            AssertHookEventsForHookType(HookType.AfterScenario);
            AssertHookEventsForHookType(HookType.BeforeStep);
            AssertHookEventsForHookType(HookType.AfterStep);
        }
        
        [Fact]
        public async Task Should_publish_scenario_skipped_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioSkipped();
            await testExecutionEngine.OnAfterLastStepAsync();
            await testExecutionEngine.OnScenarioEndAsync();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<ScenarioStartedEvent>(e =>
                                                                e.ScenarioContext.Equals(scenarioContext) &&
                                                                e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.IsAny<ScenarioSkippedEvent>()), Times.Once);
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<ScenarioFinishedEvent>(e =>
                                                                 e.ScenarioContext.Equals(scenarioContext) &&
                                                                 e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_feature_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnFeatureStartAsync(featureInfo);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<FeatureStartedEvent>(e =>
                                                               e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_feature_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnFeatureEndAsync();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<FeatureFinishedEvent>(e => 
                                                                e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public async Task Should_publish_testrun_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnTestRunStartAsync();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<TestRunStartedEvent>()), Times.Once);
        }

        [Fact]
        public async Task Should_publish_testrun_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnTestRunEndAsync();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<TestRunFinishedEvent>()), Times.Once);
        }

        private void AssertHookEventsForHookType(HookType hookType)
        {
            _testThreadExecutionEventPublisher.Verify(
                te =>
                    te.PublishEvent(It.Is<HookStartedEvent>(e => e.HookType == hookType)),
                Times.Once);
            _testThreadExecutionEventPublisher.Verify(
                te =>
                    te.PublishEvent(It.Is<HookFinishedEvent>(e => e.HookType == hookType)),
                Times.Once);
        }
    }
}
