using System;
using System.Linq;
using System.Reflection;
using System.Text;
using BoDi;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class TestRunnerFactoryTests
    {
        private readonly Mock<ITestRunner> testRunnerFake = new Mock<ITestRunner>();
        private readonly Mock<IObjectContainer> objectContainerStub = new Mock<IObjectContainer>();
        private readonly RuntimeConfiguration runtimeConfigurationStub = new RuntimeConfiguration();
        private readonly Assembly anAssembly = Assembly.GetExecutingAssembly();
        private readonly Assembly anotherAssembly = typeof(TestRunnerFactory).Assembly;

        private TestRunnerFactory CreateTestRunnerFactory()
        {
            objectContainerStub.Setup(o => o.Resolve<ITestRunner>()).Returns(testRunnerFake.Object);
            objectContainerStub.Setup(o => o.Resolve<IBindingAssemblyLoader>()).Returns(new BindingAssemblyLoader());
            return new TestRunnerFactory(objectContainerStub.Object, runtimeConfigurationStub);
        }

        [Test]
        public void Should_resolve_a_test_runner()
        {
            var factory = CreateTestRunnerFactory();

            var testRunner = factory.Create(anAssembly);
            testRunner.ShouldNotBeNull();
        }

        [Test]
        public void Should_initialize_test_runner_with_the_provided_assembly()
        {
            var factory = CreateTestRunnerFactory();
            factory.Create(anAssembly);

            testRunnerFake.Verify(tr => tr.InitializeTestRunner(It.Is<Assembly[]>(assemblies => assemblies.Contains(anAssembly))));
        }

        [Test]
        public void Should_initialize_test_runner_with_additional_step_assemblies()
        {
            var factory = CreateTestRunnerFactory();
            runtimeConfigurationStub.AddAdditionalStepAssembly(anotherAssembly);

            factory.Create(anAssembly);

            testRunnerFake.Verify(tr => tr.InitializeTestRunner(It.Is<Assembly[]>(assemblies => assemblies.Contains(anotherAssembly))));
        }

        [Test]
        public void Should_initialize_test_runner_with_the_provided_assembly_even_if_there_are_additional_ones()
        {
            var factory = CreateTestRunnerFactory();
            runtimeConfigurationStub.AddAdditionalStepAssembly(anotherAssembly);

            factory.Create(anAssembly);

            testRunnerFake.Verify(tr => tr.InitializeTestRunner(It.Is<Assembly[]>(assemblies => assemblies.Contains(anAssembly))));
        }
    }
}
