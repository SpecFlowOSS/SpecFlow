using System;
using System.Collections.Generic;
using BoDi;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Bindings;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public partial class TestExecutionEngineTests
    {
        private const string SimulatedErrorMessage = "simulated error";
        private void RegisterFailingHook(List<IHookBinding> hooks)
        {
            TimeSpan duration;
            var hookMock = CreateHookMock(hooks);
            methodBindingInvokerMock.Setup(i => i.InvokeBinding(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, out duration))
                                    .Throws(new Exception(SimulatedErrorMessage));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforetestrun()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            testExecutionEngine.OnTestRunStart();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.BeforeTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforetestrun_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(beforeTestRunEvents);
            Action act = () => testExecutionEngine.OnTestRunStart();

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_aftertestrun_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(afterTestRunEvents);
            Action act = () => testExecutionEngine.OnTestRunEnd();

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforefeature_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(beforeFeatureEvents);
            Action act = () => testExecutionEngine.OnFeatureStart(featureInfo);

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_afterfeature_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(afterFeatureEvents);
            Action act = () => testExecutionEngine.OnFeatureEnd();

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforescenario_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(beforeScenarioEvents);
            Action act = () =>
            {
                //NOTE: the exception will be re-thrown in the OnAfterLastStep
                testExecutionEngine.OnScenarioStart();
                testExecutionEngine.OnAfterLastStep(); 
            };

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_afterscenario_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(afterScenarioEvents);
            Action act = () => testExecutionEngine.OnScenarioEnd();

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_beforestep_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();
            RegisterFailingHook(beforeStepEvents);

            Action act = () =>
            {
                //NOTE: the exception will be re-thrown in the OnAfterLastStep
                testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
                testExecutionEngine.OnAfterLastStep();
            };

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
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
        public void Should_emit_runtime_plugin_test_execution_lifecycle_event_afterstep_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();
            RegisterFailingHook(afterStepEvents);

            Action act = () =>
            {
                //NOTE: the exception will be re-thrown in the OnAfterLastStep
                testExecutionEngine.Step(StepDefinitionKeyword.Given, null, "foo", null, null);
                testExecutionEngine.OnAfterLastStep();
            };

            act.Should().Throw<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RasiseExecutionLifecycleEvent(HookType.AfterStep, It.IsAny<IObjectContainer>()));
        }

    }
}
