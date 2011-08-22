using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    public abstract class AsyncTestRunnerTestBase
    {
        protected Mock<ITestRunner> mockTestRunner;
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
            mockTestRunner = new Mock<ITestRunner>();
            mockTestRunner.Setup(tr => tr.ScenarioContext).Returns(() => contextManagerStub.Object.ScenarioContext);
            mockTestRunner.Setup(tr => tr.FeatureContext).Returns(() => contextManagerStub.Object.FeatureContext);
            mockTestRunner.Setup(m => m.OnScenarioStart(It.IsAny<ScenarioInfo>())).Callback(
                (ScenarioInfo si) =>
                contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(new ScenarioContext(si, mockTestRunner.Object))
                //ObjectContainer.ScenarioContext = new ScenarioContext(si, mockTestRunner.Object)
                );
            fakeAsyncTestExecutor = new FakeAsyncTestExecutor();

            asyncTestRunner = new AsyncTestRunner(mockTestRunner.Object);
        }
    }
}