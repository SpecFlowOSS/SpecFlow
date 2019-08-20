using System.Reflection;
using System.Threading.Tasks;
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
        public async Task CreateTestRunner_should_be_able_to_create_a_testrunner()
        {
            var testRunner = await testRunnerManager.CreateTestRunnerAsync(nameof(TestRunnerManagerTest));

            testRunner.Should().NotBeNull();
            testRunner.Should().BeOfType<TestRunner>();
        }

        [Fact]
        public async Task GetTestRunner_should_be_able_to_create_a_testrunner()
        {
            var testRunner = await testRunnerManager.GetTestRunnerAsync(nameof(TestRunnerManagerTest));

            testRunner.Should().NotBeNull();
            testRunner.Should().BeOfType<TestRunner>();
        }

        [Fact]
        public async Task GetTestRunner_should_cache_instance()
        {
            var testRunner1 = await testRunnerManager.GetTestRunnerAsync(nameof(TestRunnerManagerTest));
            var testRunner2 = await testRunnerManager.GetTestRunnerAsync(nameof(TestRunnerManagerTest));


            testRunner1.Should().Be(testRunner2);
        }

        [Fact]
        public async Task Should_return_different_instances_for_different_test_ids()
        {
            var testRunner1 = await testRunnerManager.GetTestRunnerAsync(nameof(TestRunnerManagerTest) + "_0");
            var testRunner2 = await testRunnerManager.GetTestRunnerAsync(nameof(TestRunnerManagerTest) + "_1");

            testRunner1.Should().NotBe(testRunner2);
        }
    }
}