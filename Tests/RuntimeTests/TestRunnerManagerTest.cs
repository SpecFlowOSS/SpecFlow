using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class TestRunnerManagerTest
    {
        [Test]
        public void ReturnsSynchronousTestRunnerByDefault()
        {
            var testRunner = TestRunnerManager.GetTestRunner();

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void ReturnsAsyncTestRunnerIfFeatureInstanceImplementsIAsyncFeature()
        {
            var testRunner = TestRunnerManager.GetTestRunner(new AsyncFeature());

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<AsyncTestRunner>(testRunner);
        }

        [Test]
        public void ReturnsAsyncTestRunnerIfFeatureClassImplementsIAsyncFeature()
        {
            var testRunner = TestRunnerManager.GetTestRunner(typeof(AsyncFeature));

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<AsyncTestRunner>(testRunner);
        }

        [Test]
        public void ReturnsSynchronousTestRunnerIfFeatureInstanceDoesNotImplementIAsyncFeature()
        {
            var testRunner = TestRunnerManager.GetTestRunner(new SynchronousFeature());

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void ReturnsSynchronousTestRunnerIfFeatureClassDoesNotImplementIAsyncFeature()
        {
            var testRunner = TestRunnerManager.GetTestRunner(typeof(SynchronousFeature));

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        public class AsyncFeature : IAsyncFeature
        {
        }

        public class SynchronousFeature
        {
        }
    }
}