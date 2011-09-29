using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class NUnitExecutionSteps
    {
        private readonly NUnitTestExecutionDriver executionDriver;

        public NUnitExecutionSteps(NUnitTestExecutionDriver executionDriver)
        {
            this.executionDriver = executionDriver;
        }

        [When(@"I execute the tests with NUnit")]
        public void WhenIExecuteTheTestsWithNUnit()
        {
            executionDriver.Execute();
        }
    }
}
