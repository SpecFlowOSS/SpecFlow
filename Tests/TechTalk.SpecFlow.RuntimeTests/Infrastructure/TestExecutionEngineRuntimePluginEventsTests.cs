using BoDi;
using Moq;
using TechTalk.SpecFlow.Bindings;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public partial class TestExecutionEngineTests
    {
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
    }
}
