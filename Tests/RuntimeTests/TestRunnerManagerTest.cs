using System.Reflection;
using NUnit.Framework;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class TestRunnerManagerTest
    {
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly anotherAssembly = typeof(TestRunnerManager).Assembly;
        private TestRunnerManager testRunnerManager;

        [SetUp]
        public void Setup()
        {
            TestRunnerManager.Reset();

            testRunnerManager = new TestRunnerManager();
        }

        [Test]
        public void GetTestRunner_without_arguments_should_return_synchronous_TestRunner_instance()
        {
            var testRunner = TestRunnerManager.GetTestRunner();

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void GetAsyncTestRunner_should_return_synchronous_AsyncTestRunner_instance()
        {
            var testRunner = TestRunnerManager.GetAsyncTestRunner();

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<AsyncTestRunner>(testRunner);
        }

        [Test]
        public void CreateTestRunner_should_be_able_to_create_synch_runner()
        {
            var testRunner = testRunnerManager.CreateTestRunner(anAssembly, async: false);

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void CreateTestRunner_should_be_able_to_create_asynch_runner()
        {
            var testRunner = testRunnerManager.CreateTestRunner(anAssembly, async: true);

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<AsyncTestRunner>(testRunner);
        }

        [Test]
        public void GetTestRunner_should_be_able_to_create_synch_runner()
        {
            var testRunner = testRunnerManager.GetTestRunner(anAssembly, async: false);

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<TestRunner>(testRunner);
        }

        [Test]
        public void GetTestRunner_should_be_able_to_create_asynch_runner()
        {
            var testRunner = testRunnerManager.GetTestRunner(anAssembly, async: true);

            Assert.IsNotNull(testRunner);
            Assert.IsInstanceOf<AsyncTestRunner>(testRunner);
        }

        [Test]
        public void GetTestRunner_should_cache_instance()
        {
            var testRunner1 = testRunnerManager.GetTestRunner(anAssembly, async: false);
            var testRunner2 = testRunnerManager.GetTestRunner(anAssembly, async: false);

            Assert.AreEqual(testRunner1, testRunner2);
        }

        [Test]
        public void Should_return_async_instance_even_if_sync_instance_was_already_cached()
        {
            var testRunner1 = testRunnerManager.GetTestRunner(anAssembly, async: false);
            var testRunner2 = testRunnerManager.GetTestRunner(anAssembly, async: true);

            Assert.AreNotEqual(testRunner1, testRunner2);
        }

        [Test]
        public void Should_return_different_instances_for_different_assemblies()
        {
            var testRunner1 = testRunnerManager.GetTestRunner(anAssembly, async: false);
            var testRunner2 = testRunnerManager.GetTestRunner(anotherAssembly, async: false);

            Assert.AreNotEqual(testRunner1, testRunner2);
        }

        [Test]
        public void Should_provide_a_singleton_instance()
        {
            Assert.IsNotNull(TestRunnerManager.Instance);
        }
    }
}