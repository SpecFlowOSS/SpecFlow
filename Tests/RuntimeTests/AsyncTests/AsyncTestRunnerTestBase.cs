using BoDi;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    public abstract class AsyncTestRunnerTestBase
    {
        protected Mock<ITestExecutionEngine> testExecutionEngineStub;
        protected Mock<IContextManager> contextManagerStub;
        protected FakeAsyncTestExecutor fakeAsyncTestExecutor;
        protected AsyncTestRunner asyncTestRunner;

        protected ScenarioContext CurrentScenarioContext
        {
            get { return contextManagerStub.Object.ScenarioContext; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            contextManagerStub = new Mock<IContextManager>();
            testExecutionEngineStub = new Mock<ITestExecutionEngine>();
            testExecutionEngineStub.Setup(tr => tr.ScenarioContext).Returns(() => contextManagerStub.Object.ScenarioContext);
            testExecutionEngineStub.Setup(tr => tr.FeatureContext).Returns(() => contextManagerStub.Object.FeatureContext);
            testExecutionEngineStub.Setup(m => m.OnScenarioStart(It.IsAny<ScenarioInfo>())).Callback(
                (ScenarioInfo si) => contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(new ScenarioContext(si, asyncTestRunner, null))
                );
            fakeAsyncTestExecutor = new FakeAsyncTestExecutor();

            asyncTestRunner = new AsyncTestRunner(testExecutionEngineStub.Object);
        }
    }
}