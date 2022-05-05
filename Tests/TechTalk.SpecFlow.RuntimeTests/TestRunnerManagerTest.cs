using System.Reflection;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    /// <summary>
    /// Testing instance members of TestRunnerManager
    /// </summary>
    
    public class TestRunnerManagerTest
    {
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private TestRunnerManager testRunnerManager;

        public TestRunnerManagerTest()
        {
            var globalContainer = new RuntimeTestsContainerBuilder().CreateGlobalContainer(typeof(TestRunnerManagerTest).Assembly);
            testRunnerManager = globalContainer.Resolve<TestRunnerManager>();
            testRunnerManager.Initialize(anAssembly);
        }

        [Fact]
        public void CreateTestRunner_should_be_able_to_create_a_testrunner()
        {
            var testRunner = testRunnerManager.CreateTestRunner("0");

            testRunner.Should().NotBeNull();
            testRunner.Should().BeOfType<TestRunner>();
        }

        [Fact]
        public void GetTestRunner_should_be_able_to_create_a_testrunner()
        {
            var testRunner = testRunnerManager.GetTestRunner("0");

            testRunner.Should().NotBeNull();
            testRunner.Should().BeOfType<TestRunner>();
        }

        [Fact]
        public void GetTestRunner_should_cache_instance()
        {
            var testRunner1 = testRunnerManager.GetTestRunner("0");
            var testRunner2 = testRunnerManager.GetTestRunner("0");


            testRunner1.Should().Be(testRunner2);
        }

        [Fact]
        public void Should_return_different_instances_for_different_thread_ids()
        {
            var testRunner1 = testRunnerManager.GetTestRunner("0");
            var testRunner2 = testRunnerManager.GetTestRunner("1");

            testRunner1.Should().NotBe(testRunner2);
        }
    }
}