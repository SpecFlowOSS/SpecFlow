using System;
using NUnit.Framework;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    [TestFixture]
    public class AsyncTestRunnerEventsTest : AsyncTestRunnerTestBase
    {
        [Test]
        public void OnFeatureStart_DefersToInnerSynchronousTestRunner()
        {
            var featureInfo = new FeatureInfo(CultureInfoHelper.GetCultureInfo("en-us"), "title", "description");

            asyncTestRunner.OnFeatureStart(featureInfo);

            testExecutionEngineStub.Verify(m => m.OnFeatureStart(featureInfo));
        }

        [Test]
        public void OnFeatureEnd_DefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.OnFeatureEnd();

            testExecutionEngineStub.Verify(m => m.OnFeatureEnd());
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

            testExecutionEngineStub.Verify(m => m.OnScenarioStart(scenarioInfo));
        }

        [Test]
        public void OnScenarioStart_RegistersAsyncExecutorInScenarioContext()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            var scenarioInfo = new ScenarioInfo("title");
            asyncTestRunner.OnScenarioStart(scenarioInfo);

            Assert.IsNotNull(CurrentScenarioContext.Get<IAsyncTestExecutor>());
        }

        [Test]
        public void OnScenarioStart_RegistersAsyncTestRunnerInScenarioContext()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            var scenarioInfo = new ScenarioInfo("title");
            asyncTestRunner.OnScenarioStart(scenarioInfo);

            Assert.AreEqual(asyncTestRunner, CurrentScenarioContext.TestRunner);
        }

        [Test]
        public void OnScenarioEnd_DefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            asyncTestRunner.OnScenarioEnd();

            testExecutionEngineStub.Verify(m => m.OnScenarioEnd());
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