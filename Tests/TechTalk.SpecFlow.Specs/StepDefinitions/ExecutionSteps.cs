using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Specs.Drivers;
using Should;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly SpecFlowConfigurationDriver configurationDriver;
        private readonly NUnitTestExecutionDriver nUnitTestExecutionDriver;

        public ExecutionSteps(NUnitTestExecutionDriver nUnitTestExecutionDriver, SpecFlowConfigurationDriver configurationDriver)
        {
            this.nUnitTestExecutionDriver = nUnitTestExecutionDriver;
            this.configurationDriver = configurationDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            configurationDriver.UnitTestProviderName.ShouldEqual("NUnit");
            nUnitTestExecutionDriver.Execute();
        }
    }
}
