using System.Collections.Generic;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using System.Linq;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class TestRunnerFactory : ITestRunnerFactory
    {
        protected readonly IObjectContainer globalContainer;
        protected readonly RuntimeConfiguration runtimeConfiguration;
        private readonly ITestRunContainerBuilder testRunContainerBuilder;

        public TestRunnerFactory(IObjectContainer globalContainer, RuntimeConfiguration runtimeConfiguration, ITestRunContainerBuilder testRunContainerBuilder)
        {
            this.globalContainer = globalContainer;
            this.runtimeConfiguration = runtimeConfiguration;
            this.testRunContainerBuilder = testRunContainerBuilder;
        }

        public ITestRunner Create(Assembly testAssembly)
        {
            var testRunner = CreateTestRunnerInstance();

            var bindingAssemblies = new List<Assembly> { testAssembly };

            var assemblyLoader = globalContainer.Resolve<IBindingAssemblyLoader>();
            bindingAssemblies.AddRange(
                runtimeConfiguration.AdditionalStepAssemblies.Select(assemblyLoader.Load));

            testRunner.InitializeTestRunner(bindingAssemblies.ToArray());

            return testRunner;
        }

        protected virtual ITestRunner CreateTestRunnerInstance()
        {
            var testRunnerContainer = testRunContainerBuilder.CreateTestRunnerContainer(globalContainer);

            return testRunnerContainer.Resolve<ITestRunner>();
        }
    }
}