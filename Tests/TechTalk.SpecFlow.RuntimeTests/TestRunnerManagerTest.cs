using System;
using System.Reflection;
using System.Threading;
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
            var globalContainer = new ContainerBuilder().CreateGlobalContainer();
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
        public void GetTestRunner_should_return_same_instance_when_parallel_running_is_disabled_by_environment_variable()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableNames.SpecflowDisableParallelExecution,"1");
            ITestRunner testRunner1 = null;
            ITestRunner testRunner2 =null;
            var thread1 = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;                
                testRunner1 = TestRunnerManager.GetTestRunner();
            });

            var thread2 = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                testRunner2 = TestRunnerManager.GetTestRunner();
            });
            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();
            Assert.IsNotNull(testRunner1);
            Assert.IsNotNull(testRunner2);
            Assert.AreEqual(testRunner1, testRunner2);
        }
        
        [Test]        
        public void GetTestRunner_should_return_different_instances_when_executed_by_different_threads()
        {
            //This test can't run in NCrunch as when NCrunch runs the tests it will disable the ability to get different test runners for each thread 
            //as it manages the parallelisation 
            //see https://github.com/techtalk/SpecFlow/issues/638
            if (!TestEnvironmentHelper.IsBeingRunByNCrunch())
            {
                ITestRunner testRunner1 = null;
                ITestRunner testRunner2 = null;
                var thread1 = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    testRunner1 = TestRunnerManager.GetTestRunner();
                });


                var thread2 = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    testRunner2 = TestRunnerManager.GetTestRunner();
                });
                thread1.Start();
                thread2.Start();

                thread1.Join();
                thread2.Join();
                Assert.IsNotNull(testRunner1);
                Assert.IsNotNull(testRunner2);
                Assert.AreNotEqual(testRunner1, testRunner2);
            }
        }

        [Test]
        public void Should_return_different_instances_for_different_thread_ids()
        {
            var testRunner1 = testRunnerManager.GetTestRunner(threadId: 1);
            var testRunner2 = testRunnerManager.GetTestRunner(threadId: 2);

            Assert.AreNotEqual(testRunner1, testRunner2);
        }

        [Test]
        public void Should_return_same_instances_for_different_thread_ids_if_parallel_running_is_disabled_by_environment_variable()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableNames.SpecflowDisableParallelExecution, "1");
            var testRunner1 = testRunnerManager.GetTestRunner(threadId: 1);
            var testRunner2 = testRunnerManager.GetTestRunner(threadId: 2);

            Assert.AreNotEqual(testRunner1, testRunner2);
        }
    }
}