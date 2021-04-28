using System;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Events;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public partial class TestExecutionEngineTests
    {
        [Fact]
        public void Should_publish_step_started_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            SetupEventPublisher(typeof(StepStartedEvent), stepDef);
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepStartedEvent>()), Times.Once);
        }

        [Fact]
        public void Should_publish_step_binding_started_event()
        {
            var stepDef = RegisterStepDefinition().Object;

            _testThreadExecutionEventPublisher.Setup(te => te.PublishEvent(It.IsAny<IExecutionEvent>()))
                                              .Callback<IExecutionEvent>(e =>
                                              {
                                                  if(e is StepBindingStartedEvent) VerifyStepHook(e, stepDef);
                                              });

            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepBindingStartedEvent>()), Times.Once);
        }

        [Fact]
        public void Should_publish_step_binding_finished_event()
        {
            var stepDef = RegisterStepDefinition().Object;

            TimeSpan expectedDuration = TimeSpan.FromSeconds(5);

            _testThreadExecutionEventPublisher.Setup(te => te.PublishEvent(It.IsAny<IExecutionEvent>()))
                                              .Callback<IExecutionEvent>(
                                                  (e) =>
                                                  {
                                                      if (e is StepBindingFinishedEvent) VerifyStepHook(e, stepDef, expectedDuration);
                                                  });
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(stepDef, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, out expectedDuration));

            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepBindingFinishedEvent>()), Times.Once);
        }


        [Fact]
        public void Should_publish_step_finished_event()
        {
            var stepDef = RegisterStepDefinition().Object;

            _testThreadExecutionEventPublisher.Setup(te => te.PublishEvent(It.IsAny<IExecutionEvent>()))
                                              .Callback<IExecutionEvent>(e =>
                                              {
                                                  if (e is StepFinishedEvent) VerifyStepHook(e, stepDef);
                                              });

            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepFinishedEvent>()), Times.Once);
        }



        //[Fact]
        //public void Should_publish_step_events()
        //{
        //    var stepDef = RegisterStepDefinition().Object;

        //    TimeSpan expectedDuration = TimeSpan.FromSeconds(5);
        //    TimeSpan executionDuration = TimeSpan.Zero;
        //    testTracerStub.Setup(c => c.TraceStepDone(It.IsAny<BindingMatch>(), It.IsAny<object[]>(), It.IsAny<TimeSpan>()))
        //                  .Callback<BindingMatch, object[], TimeSpan>((match, arguments, duration) => executionDuration = duration);
            
        //    _testThreadExecutionEventPublisher.Setup(te => te.PublishEvent(It.IsAny<IExecutionEvent>()))
        //                                      .Callback<IExecutionEvent>(e => VerifyStepHook(e, stepDef, expectedDuration));


        //    var testExecutionEngine = CreateTestExecutionEngine();

        //    methodBindingInvokerMock.Setup(i => i.InvokeBinding(stepDef, contextManagerStub.Object, It.IsAny<object[]>(), testTracerStub.Object, out expectedDuration));

        //    testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

        //    _testThreadExecutionEventPublisher.Verify(te =>
        //                                                  te.PublishEvent(It.IsAny<StepStartedEvent>()), Times.Once);
        //    _testThreadExecutionEventPublisher.Verify(te =>
        //                                                  te.PublishEvent(It.IsAny<StepBindingStartedEvent>()), Times.Once);
        //    _testThreadExecutionEventPublisher.Verify(te =>
        //                                                  te.PublishEvent(It.IsAny<StepBindingFinishedEvent>()), Times.Once);
        //    _testThreadExecutionEventPublisher.Verify(te =>
        //                                                  te.PublishEvent(It.IsAny<StepFinishedEvent>()), Times.Once);
        //}

        

        [Fact]
        public void Should_publish_step_skipped_event()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();
            //a step will be skipped if the ScenarioExecutionStatus is not OK
            scenarioContext.ScenarioExecutionStatus = ScenarioExecutionStatus.TestError;

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<StepSkippedEvent>()), Times.Once);
        }
        
        //TODO: finish hook binding events
        [Fact]
        public void Should_publish_hook_binding_events()
        {
            HookBindingStartedEvent execEvent = null;
            _testThreadExecutionEventPublisher.Setup(te => te.PublishEvent(It.IsAny<HookBindingStartedEvent>()))
                                              .Callback<IExecutionEvent>(e => execEvent = (HookBindingStartedEvent)e);

            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            var hookMock = CreateHookMock(beforeStepEvents);

            testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);

            var expectedHook = hookMock.Object;

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<HookBindingStartedEvent>()), Times.Once);
            execEvent?.HookBinding.Should().Be(expectedHook);

            //beforeScenarioEvents;
            //afterScenarioEvents;
            //beforeStepEvents;
            //afterStepEvents;
            //beforeFeatureEvents;
            //afterFeatureEvents;
            //beforeTestRunEvents;
            //afterTestRunEvents;
            //beforeScenarioBlockEvents;
            //afterScenarioBlockEvents;
        }

        [Fact]
        public void Should_publish_scenario_started_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            SetupEventPublisher(typeof(ScenarioStartedEvent), stepDef);
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            testExecutionEngine.OnScenarioStart();
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<ScenarioStartedEvent>()), Times.Once);
        }

        [Fact]
        public void Should_publish_scenario_finished_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            SetupEventPublisher(typeof(ScenarioFinishedEvent), stepDef);
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);
            
            testExecutionEngine.OnScenarioStart();
            testExecutionEngine.OnAfterLastStep();
            testExecutionEngine.OnScenarioEnd();
            
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<ScenarioFinishedEvent>()), Times.Once);
        }

        [Fact]
        public void Should_publish_scenario_skipped_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            SetupEventPublisher(typeof(ScenarioSkippedEvent), stepDef);
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnScenarioInitialize(scenarioInfo);

            testExecutionEngine.OnScenarioSkipped();
            testExecutionEngine.OnAfterLastStep();
            testExecutionEngine.OnScenarioEnd();

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<ScenarioStartedEvent>()), Times.Once);
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<ScenarioSkippedEvent>()), Times.Once);
            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<ScenarioFinishedEvent>()), Times.Once);
        }

        [Fact]
        public void Should_publish_feature_finished_event()
        {
            var stepDef = RegisterStepDefinition().Object;
            SetupEventPublisher(typeof(FeatureFinishedEvent), stepDef);
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnFeatureEnd();

            _testThreadExecutionEventPublisher.Verify(te =>
                                                          te.PublishEvent(It.IsAny<FeatureFinishedEvent>()), Times.Once);
        }


        private void VerifyStepHook(IExecutionEvent executionEvent, IStepDefinitionBinding stepDefinitionBinding = null, TimeSpan duration = default)
        {
            switch (executionEvent)
            {
                case StepStartedEvent stepStartedEvent:
                    stepStartedEvent.ScenarioContext.Should().BeEquivalentTo(scenarioContext);
                    stepStartedEvent.FeatureContext.Should().BeEquivalentTo(featureContainer.Resolve<FeatureContext>());
                    stepStartedEvent.StepContext.Should().BeEquivalentTo(contextManagerStub.Object.StepContext);
                    break;
                case StepFinishedEvent finished:
                    finished.ScenarioContext.Should().BeEquivalentTo(scenarioContext);
                    finished.FeatureContext.Should().BeEquivalentTo(featureContainer.Resolve<FeatureContext>());
                    finished.StepContext.Should().BeEquivalentTo(contextManagerStub.Object.StepContext);
                    break;
                case StepSkippedEvent stepSkippedEvent: break;
                case ScenarioSkippedEvent stepSkippedEvent: break;
                case StepBindingStartedEvent stepBindingStartedEvent:
                    stepBindingStartedEvent.StepDefinitionBinding.Should().Be(stepDefinitionBinding);
                    break;
                case StepBindingFinishedEvent stepBindingFinishedEvent:
                    stepBindingFinishedEvent.StepDefinitionBinding.Should().Be(stepDefinitionBinding);
                    stepBindingFinishedEvent.Duration.Should().Be(duration);
                    break;
                case ScenarioStartedEvent scenarioStarted:
                    scenarioStarted.ScenarioContext.Should().BeEquivalentTo(scenarioContext);
                    scenarioStarted.FeatureContext.Should().BeEquivalentTo(featureContainer.Resolve<FeatureContext>());
                    break;
                case ScenarioFinishedEvent scenarioFinished:
                    scenarioFinished.ScenarioContext.Should().BeEquivalentTo(scenarioContext);
                    scenarioFinished.FeatureContext.Should().BeEquivalentTo(featureContainer.Resolve<FeatureContext>());
                    break;
                default: break;
            }
        }

        private void SetupEventPublisher(Type eventType, IStepDefinitionBinding stepDefinitionBinding = null, TimeSpan duration = default)
        {
            _testThreadExecutionEventPublisher.Setup(te => te.PublishEvent(It.IsAny<IExecutionEvent>()))
                                              .Callback<IExecutionEvent>(e =>
                                              {
                                                  if (e.GetType() == eventType) VerifyStepHook(e, stepDefinitionBinding, duration);
                                              });
        }
    }
}
