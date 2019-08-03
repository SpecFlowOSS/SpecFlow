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
        private readonly Assembly thisAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly anotherAssembly = typeof(TestRunnerManager).Assembly;

        public TestRunnerManagerStaticApiTest()
        {
            TestRunnerManager.Reset();
        }

        [Fact]
        public void GetTestRunner_without_arguments_should_return_TestRunner_instance()
        {
            var testRunner = TestRunnerManager.GetTestRunner(containerBuilder: new RuntimeTestsContainerBuilder());

            testRunner.Should().NotBeNull();
            testRunner.Should().BeOfType<TestRunner>();
        }

        [Fact]
        public void GetTestRunner_should_return_different_instances_for_different_assemblies()
        {
            var testRunner1 = TestRunnerManager.GetTestRunner(thisAssembly, containerBuilder: new RuntimeTestsContainerBuilder());
            var testRunner2 = TestRunnerManager.GetTestRunner(anotherAssembly, containerBuilder: new RuntimeTestsContainerBuilder());

            testRunner1.Should().NotBe(testRunner2);
        }

        [Fact]
        public void GetTestRunnerManager_without_arguments_should_return_an_instance_for_the_calling_assembly()
        {
            var testRunnerManager = TestRunnerManager.GetTestRunnerManager(containerBuilder: new RuntimeTestsContainerBuilder());

            testRunnerManager.Should().NotBeNull();
            testRunnerManager.TestAssembly.Should().BeSameAs(thisAssembly);
        }

        [Fact]
        public void GetTestRunnerManager_should_return_null_when_called_with_no_create_flag_and_there_was_no_instance_created_yet()
        {
            TestRunnerManager.Reset();

            var testRunnerManager = TestRunnerManager.GetTestRunnerManager(createIfMissing: false, containerBuilder: new RuntimeTestsContainerBuilder());

            testRunnerManager.Should().BeNull();
        }

        [Binding]
        public class AfterTestRunTestBinding
        {
            public static int AfterTestRunCallCount = 0;

            [AfterTestRun]
            public static void AfterTestRun()
            {
                AfterTestRunCallCount++;
            }
        }

        [Binding]
        public class BeforeTestRunTestBinding
        {
            public static int BeforeTestRunCallCount = 0;

            [BeforeTestRun]
            public static void BeforeTestRun()
            {
                BeforeTestRunCallCount++;
            }
        }

        [Fact]
        public void OnTestRunEnd_should_fire_AfterTestRun_events()
        {
            // make sure a test runner is initialized
            TestRunnerManager.GetTestRunner(thisAssembly, containerBuilder: new RuntimeTestsContainerBuilder());

            AfterTestRunTestBinding.AfterTestRunCallCount = 0; //reset
            TestRunnerManager.OnTestRunEnd(thisAssembly);

            AfterTestRunTestBinding.AfterTestRunCallCount.Should().Be(1);
        }

        [Fact]
        public void OnTestRunEnd_without_arguments_should_fire_AfterTestRun_events_for_calling_assembly()
        {
            // make sure a test runner is initialized
            TestRunnerManager.GetTestRunner(thisAssembly, containerBuilder: new RuntimeTestsContainerBuilder());

            AfterTestRunTestBinding.AfterTestRunCallCount = 0; //reset
            TestRunnerManager.OnTestRunEnd();

            AfterTestRunTestBinding.AfterTestRunCallCount.Should().Be(1);
        }

        [Fact]
        public void OnTestRunEnd_should_not_fire_AfterTestRun_events_multiple_times()
        {
            // make sure a test runner is initialized
            TestRunnerManager.GetTestRunner(thisAssembly, containerBuilder: new RuntimeTestsContainerBuilder());

            AfterTestRunTestBinding.AfterTestRunCallCount = 0; //reset
            TestRunnerManager.OnTestRunEnd(thisAssembly);
            TestRunnerManager.OnTestRunEnd(thisAssembly);

            AfterTestRunTestBinding.AfterTestRunCallCount.Should().Be(1);
        }

        [Fact]
        public void OnTestRunStart_should_fire_BeforeTestRun_events()
        {
            BeforeTestRunTestBinding.BeforeTestRunCallCount = 0; //reset
            TestRunnerManager.OnTestRunStart(thisAssembly, containerBuilder: new RuntimeTestsContainerBuilder());

            BeforeTestRunTestBinding.BeforeTestRunCallCount.Should().Be(1);
        }

        [Fact]
        public void OnTestRunStart_without_arguments_should_fire_BeforeTestRun_events_for_calling_assembly()
        {
            BeforeTestRunTestBinding.BeforeTestRunCallCount = 0; //reset
            TestRunnerManager.OnTestRunStart(containerBuilder: new RuntimeTestsContainerBuilder());

            BeforeTestRunTestBinding.BeforeTestRunCallCount.Should().Be(1);
        }

        [Fact]
        public void OnTestRunStart_should_not_fire_BeforeTestRun_events_multiple_times()
        {
            BeforeTestRunTestBinding.BeforeTestRunCallCount = 0; //reset
            TestRunnerManager.OnTestRunStart(thisAssembly, containerBuilder: new RuntimeTestsContainerBuilder());
            TestRunnerManager.OnTestRunStart(thisAssembly);

            BeforeTestRunTestBinding.BeforeTestRunCallCount.Should().Be(1);
        }

        //[Fact]
        //public void DomainUnload_event_should_not_fire_AfterTestRun_events_multiple_times_after_OnTestRunEnd()
        //{
        //    // make sure a test runner is initialized
        //    TestRunnerManager.GetTestRunner(thisAssembly);

        //    AfterTestRunTestBinding.AfterTestRunCallCount = 0; //reset
        //    TestRunnerManager.OnTestRunEnd(thisAssembly);

        //    // simulating DomainUnload event
        //    var trm = (TestRunnerManager)TestRunnerManager.GetTestRunnerManager(thisAssembly);
        //    trm.OnDomainUnload();

        //    AfterTestRunTestBinding.AfterTestRunCallCount.Should().Be(1);
        //}
    }
}
