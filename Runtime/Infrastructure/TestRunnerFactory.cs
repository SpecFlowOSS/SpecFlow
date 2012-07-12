using System.Collections.Generic;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using System.Linq;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class TestRunnerFactory : ITestRunnerFactory
    {
        protected readonly IObjectContainer objectContainer;
        protected readonly RuntimeConfiguration runtimeConfiguration;

        public TestRunnerFactory(IObjectContainer objectContainer, RuntimeConfiguration runtimeConfiguration)
        {
            this.objectContainer = objectContainer;
            this.runtimeConfiguration = runtimeConfiguration;
        }

        public ITestRunner Create(Assembly testAssembly)
        {
            var testRunner = CreateTestRunnerInstance();

            var bindingAssemblies = new List<Assembly> { testAssembly };

            var assemblyLoader = objectContainer.Resolve<IBindingAssemblyLoader>();
            bindingAssemblies.AddRange(
                runtimeConfiguration.AdditionalStepAssemblies.Select(assemblyLoader.Load));

            testRunner.InitializeTestRunner(bindingAssemblies.ToArray());

            return testRunner;
        }

        protected virtual ITestRunner CreateTestRunnerInstance()
        {
            return objectContainer.Resolve<ITestRunner>();
        }
    }
}