using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class TestRunnerManagerStaticApiTest
    {
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly anotherAssembly = typeof(TestRunnerManager).Assembly;

        public TestRunnerManagerStaticApiTest()
        {
            TestRunnerManager.Reset();
        }

        [Fact]
        public void GetTestRunner_without_arguments_should_return_TestRunner_instance()
        {
            var testRunner = TestRunnerManager.GetTestRunner();

            testRunner.Should().NotBeNull();
            testRunner.Should().BeOfType<TestRunner>();
        }

        [Fact]
        public void Should_return_different_instances_for_different_assemblies()
        {
            var testRunner1 = TestRunnerManager.GetTestRunner(anAssembly);
            var testRunner2 = TestRunnerManager.GetTestRunner(anotherAssembly);

            testRunner1.Should().NotBe(testRunner2);
        }
    }
}
