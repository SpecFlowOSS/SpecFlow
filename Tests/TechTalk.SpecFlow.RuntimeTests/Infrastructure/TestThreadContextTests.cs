using BoDi;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class TestThreadContextTests : StepExecutionTestsBase
    {
        public ContextManager CreateContextManager(IObjectContainer testThreadContainer = null)
        {
            return new ContextManager(MockRepository.Stub<ITestTracer>(), testThreadContainer ?? TestThreadContainer, ContainerBuilderStub);
        }

        [Test]
        public void Should_ContextManager_initialize_TestThreadContext_when_constructed()
        {
            var contextManager = CreateContextManager();

            TestThreadContext result = contextManager.TestThreadContext;

            result.Should().NotBeNull();
            result.Should().BeSameAs(TestThreadContainer.Resolve<TestThreadContext>());
        }

        [Test]
        public void Should_expose_the_test_thread_container()
        {
            TestThreadContext result = ContextManagerStub.TestThreadContext;

            result.Should().NotBeNull();
            result.TestThreadContainer.Should().BeSameAs(TestThreadContainer);
        }

        [Test]
        public void Should_disposing_event_fired_when_test_thread_container_disposes()
        {
            bool wasDisposingFired = false;
            ContextManagerStub.TestThreadContext.Should().NotBeNull();
            ContextManagerStub.TestThreadContext.Disposing += context =>
            {
                context.Should().BeSameAs(ContextManagerStub.TestThreadContext);
                wasDisposingFired = true;
            };

            TestThreadContainer.Dispose();

            wasDisposingFired.Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_resolve_from_scenario_container()
        {
            // this basically tests the special registration in DefaultDependencyProvider

            var containerBuilder = new ContainerBuilder();
            var testThreadContainer = containerBuilder.CreateTestThreadContainer(containerBuilder.CreateGlobalContainer());
            var contextManager = CreateContextManager(testThreadContainer);
            contextManager.InitializeFeatureContext(new FeatureInfo(FeatureLanguage, "test feature", null));
            contextManager.InitializeScenarioContext(new ScenarioInfo("test scenario"));

            contextManager.TestThreadContext.Should().NotBeNull();

            var ctxFromScenarioContext = contextManager.ScenarioContext.ScenarioContainer.Resolve<TestThreadContext>();

            ctxFromScenarioContext.Should().BeSameAs(contextManager.TestThreadContext);
        }
    }
}
