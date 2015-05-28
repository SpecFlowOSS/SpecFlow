using System;
using System.Linq;
using System.Reflection;
using BoDi;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.RuntimeTests.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class TestRunnerManagerRunnerCreationTests
    {
        private readonly Mock<ITestRunner> testRunnerFake = new Mock<ITestRunner>();
        private readonly Mock<IObjectContainer> objectContainerStub = new Mock<IObjectContainer>();
        private readonly Mock<IObjectContainer> globalObjectContainerStub = new Mock<IObjectContainer>();
        private readonly RuntimeConfiguration runtimeConfigurationStub = new RuntimeConfiguration();
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly anotherAssembly = typeof(TestRunnerManager).Assembly;

        private TestRunnerManager CreateTestRunnerFactory()
        {
            objectContainerStub.Setup(o => o.Resolve<ITestRunner>()).Returns(testRunnerFake.Object);
            globalObjectContainerStub.Setup(o => o.Resolve<IBindingAssemblyLoader>()).Returns(new BindingAssemblyLoader());
            
            Mock<ITestRunContainerBuilder> testRunContainerBuilderStub = new Mock<ITestRunContainerBuilder>();
            testRunContainerBuilderStub.Setup(b => b.CreateTestRunnerContainer(It.IsAny<IObjectContainer>()))
                .Returns(objectContainerStub.Object);

            var testRunnerManager = new TestRunnerManager(globalObjectContainerStub.Object, testRunContainerBuilderStub.Object, runtimeConfigurationStub);
            testRunnerManager.Initialize(anAssembly);
            return testRunnerManager;
        }

        [Test]
        public void Should_resolve_a_test_runner()
        {
            var factory = CreateTestRunnerFactory();

            var testRunner = factory.CreateTestRunner();
            testRunner.Should().NotBeNull();
        }

        [Test]
        public void Should_initialize_test_runner_with_the_provided_assembly()
        {
            var factory = CreateTestRunnerFactory();
            factory.CreateTestRunner();

            testRunnerFake.Verify(tr => tr.InitializeTestRunner(It.Is<Assembly[]>(assemblies => assemblies.Contains(anAssembly))));
        }

        [Test]
        public void Should_initialize_test_runner_with_additional_step_assemblies()
        {
            var factory = CreateTestRunnerFactory();
            runtimeConfigurationStub.AddAdditionalStepAssembly(anotherAssembly);

            factory.CreateTestRunner();

            testRunnerFake.Verify(tr => tr.InitializeTestRunner(It.Is<Assembly[]>(assemblies => assemblies.Contains(anotherAssembly))));
        }

        [Test]
        public void Should_initialize_test_runner_with_the_provided_assembly_even_if_there_are_additional_ones()
        {
            var factory = CreateTestRunnerFactory();
            runtimeConfigurationStub.AddAdditionalStepAssembly(anotherAssembly);

            factory.CreateTestRunner();

            testRunnerFake.Verify(tr => tr.InitializeTestRunner(It.Is<Assembly[]>(assemblies => assemblies.Contains(anAssembly))));
        }
    }
}
