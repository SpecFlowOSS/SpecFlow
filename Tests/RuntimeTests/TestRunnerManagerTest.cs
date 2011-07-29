using NUnit.Framework;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class TestRunnerManagerTest
    {
        [Test]
        public void GetTestRunner_ReturnsSynchronousTestRunner()
        {
            var testRunner = TestRunnerManager.GetTestRunner();

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void GetAsyncTestRunner_ReturnsSynchronousAsyncTestRunner()
        {
            var testRunner = TestRunnerManager.GetAsyncTestRunner();

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<AsyncTestRunner>(testRunner);
        }
    }
}