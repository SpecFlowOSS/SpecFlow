using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var hookMock = CreateHookMock(hooks);
            methodBindingInvokerMock.Setup(i => i.InvokeBindingAsync(hookMock.Object, contextManagerStub.Object, null, testTracerStub.Object, It.IsAny<DurationHolder>()))
                                    .ThrowsAsync(new Exception(SimulatedErrorMessage));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforetestrun()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnTestRunStartAsync();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforetestrun_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(beforeTestRunEvents);
            Func<Task> act = async () => await testExecutionEngine.OnTestRunStartAsync();

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_aftertestrun()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnTestRunEndAsync();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_aftertestrun_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(afterTestRunEvents);
            Func<Task> act = async () => await testExecutionEngine.OnTestRunEndAsync();

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterTestRun, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforefeature()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnFeatureStartAsync(featureInfo);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeFeature, It.IsAny<IObjectContainer>()));
        }
        
        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforefeature_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(beforeFeatureEvents);
            Func<Task> act = async () => await testExecutionEngine.OnFeatureStartAsync(featureInfo);

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeFeature, It.IsAny<IObjectContainer>()));
        }


        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_afterfeature()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnFeatureEndAsync();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterFeature, It.IsAny<IObjectContainer>()));
        }
        
        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_afterfeature_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(afterFeatureEvents);
            Func<Task> act = async () => await testExecutionEngine.OnFeatureEndAsync();

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterFeature, It.IsAny<IObjectContainer>()));
        }


        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforescenario()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnScenarioStartAsync();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeScenario, It.IsAny<IObjectContainer>()));
        }
        
        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforescenario_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(beforeScenarioEvents);
            Func<Task> act = async () =>
            {
                //NOTE: the exception will be re-thrown in the OnAfterLastStep
                await testExecutionEngine.OnScenarioStartAsync();
                await testExecutionEngine.OnAfterLastStepAsync(); 
            };

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeScenario, It.IsAny<IObjectContainer>()));
        }


        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_afterscenario()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            await testExecutionEngine.OnScenarioEndAsync();

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterScenario, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_afterscenario_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();

            RegisterFailingHook(afterScenarioEvents);
            Func<Task> act = async () => await testExecutionEngine.OnScenarioEndAsync();

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterScenario, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforestep()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeStep, It.IsAny<IObjectContainer>()));
        }
        
        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_beforestep_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();
            RegisterFailingHook(beforeStepEvents);

            Func<Task> act = async () =>
            {
                //NOTE: the exception will be re-thrown in the OnAfterLastStep
                await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
                await testExecutionEngine.OnAfterLastStepAsync();
            };

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.BeforeStep, It.IsAny<IObjectContainer>()));
        }

        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_afterstep()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();

            await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);

            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterStep, It.IsAny<IObjectContainer>()));
        }
        
        [Fact]
        public async Task Should_emit_runtime_plugin_test_execution_lifecycle_event_afterstep_after_hook_error_and_throw_error()
        {
            var testExecutionEngine = CreateTestExecutionEngine();
            RegisterStepDefinition();
            RegisterFailingHook(afterStepEvents);

            Func<Task> act = async () =>
            {
                //NOTE: the exception will be re-thrown in the OnAfterLastStep
                await testExecutionEngine.StepAsync(StepDefinitionKeyword.Given, null, "foo", null, null);
                await testExecutionEngine.OnAfterLastStepAsync();
            };

            await act.Should().ThrowAsync<Exception>().WithMessage(SimulatedErrorMessage);
            _runtimePluginTestExecutionLifecycleEventEmitter.Verify(e => e.RaiseExecutionLifecycleEvent(HookType.AfterStep, It.IsAny<IObjectContainer>()));
        }

    }
}
