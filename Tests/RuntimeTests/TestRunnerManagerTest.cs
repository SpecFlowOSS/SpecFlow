using System.Reflection;
using NUnit.Framework;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    /// <summary>
    /// Testing instance members of TestRunnerManager
    /// </summary>
    [TestFixture]
    public class TestRunnerManagerTest
    {
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private TestRunnerManager testRunnerManager;

        [SetUp]
        public void Setup()
        {
            var globalContainer = new TestRunContainerBuilder().CreateContainer();
            testRunnerManager = globalContainer.Resolve<TestRunnerManager>();
            testRunnerManager.Initialize(anAssembly);
        }

        [Test]
        public void CreateTestRunner_should_be_able_to_create_a_testrunner()
        {
            var testRunner = testRunnerManager.CreateTestRunner(0);

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void GetTestRunner_should_be_able_to_create_a_testrunner()
        {
            var testRunner = testRunnerManager.GetTestRunner(0);

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void GetTestRunner_should_cache_instance()
        {
            var testRunner1 = testRunnerManager.GetTestRunner(threadId: 0);
            var testRunner2 = testRunnerManager.GetTestRunner(threadId: 0);

            Assert.AreEqual(testRunner1, testRunner2);
        }

        [Test]
        public void Should_return_different_instances_for_different_thread_ids()
        {
            var testRunner1 = testRunnerManager.GetTestRunner(threadId: 1);
            var testRunner2 = testRunnerManager.GetTestRunner(threadId: 2);

            Assert.AreNotEqual(testRunner1, testRunner2);
        }
    }
}