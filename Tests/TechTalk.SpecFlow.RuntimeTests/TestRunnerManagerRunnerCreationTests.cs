using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BoDi;
using FluentAssertions;
using Moq;
using Xunit;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.RuntimeTests.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class TestRunnerManagerRunnerCreationTests
    {
        private readonly Mock<ITestRunner> testRunnerFake = new Mock<ITestRunner>();
        private readonly Mock<IObjectContainer> objectContainerStub = new Mock<IObjectContainer>();
        private readonly Mock<IObjectContainer> globalObjectContainerStub = new Mock<IObjectContainer>();
        private readonly SpecFlow.Configuration.SpecFlowConfiguration _specFlowConfigurationStub = ConfigurationLoader.GetDefault();
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly anotherAssembly = typeof(TestRunnerManager).Assembly;

        private TestRunnerManager CreateTestRunnerFactory()
        {
            objectContainerStub.Setup(o => o.Resolve<ITestRunner>()).Returns(testRunnerFake.Object);
            globalObjectContainerStub.Setup(o => o.Resolve<IBindingAssemblyLoader>()).Returns(new BindingAssemblyLoader());
            globalObjectContainerStub.Setup(o => o.Resolve<ITraceListenerQueue>()).Returns(new Mock<ITraceListenerQueue>().Object);
            
            var testRunContainerBuilderStub = new Mock<IContainerBuilder>();
            testRunContainerBuilderStub.Setup(b => b.CreateTestThreadContainer(It.IsAny<IObjectContainer>()))
                .Returns(objectContainerStub.Object);

            var runtimeBindingRegistryBuilderMock = new Mock<IRuntimeBindingRegistryBuilder>();

            var testRunnerManager = new TestRunnerManager(globalObjectContainerStub.Object, testRunContainerBuilderStub.Object, _specFlowConfigurationStub, runtimeBindingRegistryBuilderMock.Object,
                Mock.Of<ITestTracer>());
            testRunnerManager.Initialize(anAssembly);
            return testRunnerManager;
        }

        [Fact]
        public void Should_resolve_a_test_runner()
        {
            var factory = CreateTestRunnerFactory();

            var testRunner = factory.CreateTestRunner(0);
            testRunner.Should().NotBeNull();
        }

        [Fact]
        public void Should_initialize_test_runner_with_the_provided_assembly()
        {
            var factory = CreateTestRunnerFactory();
            factory.CreateTestRunner(0);

            factory.IsTestRunInitialized.Should().BeTrue();
        }

        [Fact]
        public void Should_initialize_test_runner_with_additional_step_assemblies()
        {
            var factory = CreateTestRunnerFactory();
            _specFlowConfigurationStub.AddAdditionalStepAssembly(anotherAssembly);

            factory.CreateTestRunner(0);

            factory.IsTestRunInitialized.Should().BeTrue();
        }

        [Fact]
        public void Should_initialize_test_runner_with_the_provided_assembly_even_if_there_are_additional_ones()
        {
            var factory = CreateTestRunnerFactory();
            _specFlowConfigurationStub.AddAdditionalStepAssembly(anotherAssembly);

            factory.CreateTestRunner(0);

            factory.IsTestRunInitialized.Should().BeTrue();
        }


        [Fact]
        public void Should_resolve_a_test_runner_specific_test_tracer()
        {
            //This test can't run in NCrunch as when NCrunch runs the tests it will disable the ability to get different test runners for each thread 
            //as it manages the parallelisation 
            //see https://github.com/techtalk/SpecFlow/issues/638
            if (!TestEnvironmentHelper.IsBeingRunByNCrunch())
            {
                var testRunner1 = TestRunnerManager.GetTestRunner(anAssembly, 0, new RuntimeTestsContainerBuilder());
                testRunner1.OnFeatureStart(new FeatureInfo(new CultureInfo("en-US", false), string.Empty, "sds", "sss"));
                testRunner1.OnScenarioInitialize(new ScenarioInfo("foo", "foo_desc", null, null));
                testRunner1.OnScenarioStart();
                var tracer1 = testRunner1.ScenarioContext.ScenarioContainer.Resolve<ITestTracer>();

                var testRunner2 = TestRunnerManager.GetTestRunner(anAssembly, 1, new RuntimeTestsContainerBuilder());
                testRunner2.OnFeatureStart(new FeatureInfo(new CultureInfo("en-US", false), string.Empty, "sds", "sss"));
                testRunner2.OnScenarioInitialize(new ScenarioInfo("foo", "foo_desc", null, null));
                testRunner1.OnScenarioStart();
                var tracer2 = testRunner2.ScenarioContext.ScenarioContainer.Resolve<ITestTracer>();

                tracer1.Should().NotBeSameAs(tracer2);
            }       
        }
    }
}
