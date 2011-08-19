using System.Collections.Generic;
using System.Reflection;
using MiniDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class TestRunnerFactory : ITestRunnerFactory
    {
        private readonly IObjectContainer objectContainer;
        private readonly RuntimeConfiguration runtimeConfiguration;

        public TestRunnerFactory(IObjectContainer objectContainer, RuntimeConfiguration runtimeConfiguration)
        {
            this.objectContainer = objectContainer;
            this.runtimeConfiguration = runtimeConfiguration;
        }

        public ITestRunner Create(Assembly testAssembly)
        {
            var testRunner = objectContainer.Resolve<ITestRunner>();

            var bindingAssemblies = new List<Assembly> { testAssembly };
            bindingAssemblies.AddRange(runtimeConfiguration.AdditionalStepAssemblies);

            testRunner.InitializeTestRunner(bindingAssemblies.ToArray());

            return testRunner;
        }
    }
}