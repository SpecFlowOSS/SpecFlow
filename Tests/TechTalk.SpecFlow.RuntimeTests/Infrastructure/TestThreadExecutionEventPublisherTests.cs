using System;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Events;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public partial class TestExecutionEngineTests
    {
        [Fact]
        public void Should_publish_step_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                    te.PublishEvent(It.Is<StepStartedEvent>(e => e.ScenarioContext.Equals(scenarioContext) &&
                                                                 e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()) &&
                                                                 e.StepContext.Equals(contextManagerStub.Object.StepContext))), 
                                                      Times.Once);
        }

        [Fact]
        public void Should_publish_step_binding_started_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<StepBindingStartedEvent>(e => 
                                                                   e.StepDefinitionBinding.Equals(stepDef))),
                                                      Times.Once);
        }

        [Fact]
        public void Should_publish_step_binding_finished_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(stepDef, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, out expectedDuration));
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<StepBindingFinishedEvent>(e =>
                                                                    e.StepDefinitionBinding.Equals(stepDef) && 
                                                                    e.Duration.Equals(expectedDuration))),
                                                      Times.Once);
        }
        
        [Fact]
        public void Should_publish_step_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<StepFinishedEvent>(e => 
                                                             e.ScenarioContext.Equals(scenarioContext) &&
                                                             e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()) &&
                                                             e.StepContext.Equals(contextManagerStub.Object.StepContext))),
                                                      Times.Once);
        }
        
        [Fact]
        public void Should_publish_step_skipped_event()
        {
            RegisterStepDefinition();
            var testExecutionEngine = CreateTestExecutionEngine();
            //a step will be skipped if the ScenarioExecutionStatus is not OK
            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepSkippedEvent>()), Times.Once);
        }
        
        [Fact]
        public void Should_publish_hook_binding_events()
        {
            var hookType = HookType.AfterScenario;
            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
            var expectedHookBinding = new HookBinding(new Mock<IBindingMethod>().Object, hookType, null, 1);
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(expectedHookBinding, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, out expectedDuration));
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.InvokeHook(methodBindingInvokerMock.Object, expectedHookBinding, hookType);

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
        public void Should_publish_scenario_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioStart();

            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<ScenarioStartedEvent>(e =>
                                                                e.ScenarioContext.Equals(scenarioContext) &&
                                                                e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public void Should_publish_scenario_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioStart();
            testExecutionEngine.OnAfterLastStep();
            testExecutionEngine.OnScenarioEnd();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<ScenarioFinishedEvent>(e =>
                                                                 e.ScenarioContext.Equals(scenarioContext) &&
                                                                 e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public void Should_publish_hook_started_finished_events()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            testExecutionEngine.OnTestRunStart();
            testExecutionEngine.OnFeatureStart(featureInfo);

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioStart();
            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            testExecutionEngine.OnAfterLastStep();
            testExecutionEngine.OnScenarioEnd();

            testExecutionEngine.OnFeatureEnd();
            testExecutionEngine.OnTestRunEnd();

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
        public void Should_publish_scenario_skipped_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioSkipped();
            testExecutionEngine.OnAfterLastStep();
            testExecutionEngine.OnScenarioEnd();
            
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
        public void Should_publish_feature_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnFeatureStart(featureInfo);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<FeatureStartedEvent>(e =>
                                                               e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public void Should_publish_feature_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnFeatureEnd();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                te.PublishEvent(It.Is<FeatureFinishedEvent>(e => 
                                                                e.FeatureContext.Equals(featureContainer.Resolve<FeatureContext>()))),
                                                      Times.Once);
        }

        [Fact]
        public void Should_publish_testrun_started_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnTestRunStart();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<TestRunStartedEvent>()), Times.Once);
        }

        [Fact]
        public void Should_publish_testrun_finished_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnTestRunEnd();
            
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
