using System;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    [TestFixture]
    public class AsyncTestRunnerEventsTest
    {
        private Mock<ITestRunner> mockTestRunner;
        private FakeAsyncTestExecutor fakeAsyncTestExecutor;
        private AsyncTestRunner asyncTestRunner;

        [SetUp]
        public void SetUp()
        {
            mockTestRunner = new Mock<ITestRunner>();
            mockTestRunner.Setup(m => m.OnScenarioStart(It.IsAny<ScenarioInfo>())).Callback(
                (ScenarioInfo si) => ObjectContainer.ScenarioContext = new ScenarioContext(si, mockTestRunner.Object));
            fakeAsyncTestExecutor = new FakeAsyncTestExecutor();

            asyncTestRunner = new AsyncTestRunner(mockTestRunner.Object);
        }

        [TearDown]
        public void TearDown()
        {
            //ObjectContainer.AsyncContext = null;
        }

        [Test]
        public void OnFeatureStart_DefersToInnerSynchronousTestRunner()
        {
            var featureInfo = new FeatureInfo(CultureInfoHelper.GetCultureInfo("en-us"), "title", "description");

            asyncTestRunner.OnFeatureStart(featureInfo);

            mockTestRunner.Verify(m => m.OnFeatureStart(featureInfo));
        }

        [Test]
        public void OnFeatureEnd_DefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.OnFeatureEnd();

            mockTestRunner.Verify(m => m.OnFeatureEnd());
        }

        [Test]
        public void OnScenarioStart_ThrowsIfRegisterAsyncTestExecutorWasNotCalled()
        {
            Assert.Throws(typeof (InvalidOperationException),
                          () => asyncTestRunner.OnScenarioStart(new ScenarioInfo("title")), 
                          "Expected OnScenarioStart to fail if AsyncContext not set");
        }

        [Test]
        public void OnScenarioStart_DefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            var scenarioInfo = new ScenarioInfo("title");
            asyncTestRunner.OnScenarioStart(scenarioInfo);

            mockTestRunner.Verify(m => m.OnScenarioStart(scenarioInfo));
        }

        [Test]
        public void OnScenarioStart_RegistersAsyncExecutorInScenarioContext()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            var scenarioInfo = new ScenarioInfo("title");
            asyncTestRunner.OnScenarioStart(scenarioInfo);

            Assert.IsNotNull(ScenarioContext.Current.Get<IAsyncTestExecutor>());
        }

        [Test]
        public void OnScenarioStart_RegistersAsyncTestRunnerInScenarioContext()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            var scenarioInfo = new ScenarioInfo("title");
            asyncTestRunner.OnScenarioStart(scenarioInfo);

            Assert.AreEqual(asyncTestRunner, ScenarioContext.Current.TestRunner);
        }

        [Test]
        public void OnScenarioEnd_DefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            asyncTestRunner.OnScenarioEnd();

            mockTestRunner.Verify(m => m.OnScenarioEnd());
        }

        [Test]
        public void OnScenarioEnd_ClearsAsyncExecutor()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            asyncTestRunner.OnScenarioEnd();

            Assert.Throws(typeof (InvalidOperationException), () => asyncTestRunner.AsyncTestExecutor.ToString());
        }
    }
}