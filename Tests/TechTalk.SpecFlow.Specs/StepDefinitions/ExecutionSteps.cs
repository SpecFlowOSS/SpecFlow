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
        private readonly ProjectSteps projectSteps;
        private readonly SpecFlowConfigurationDriver configurationDriver;
        private readonly NUnitTestExecutionDriver nUnitTestExecutionDriver;
        private readonly MsTestTestExecutionDriver msTestTestExecutionDriver;

        public ExecutionSteps(NUnitTestExecutionDriver nUnitTestExecutionDriver, SpecFlowConfigurationDriver configurationDriver, MsTestTestExecutionDriver msTestTestExecutionDriver, ProjectSteps projectSteps)
        {
            this.nUnitTestExecutionDriver = nUnitTestExecutionDriver;
            this.projectSteps = projectSteps;
            this.msTestTestExecutionDriver = msTestTestExecutionDriver;
            this.configurationDriver = configurationDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            configurationDriver.UnitTestProviderName.ShouldEqual("NUnit");

            projectSteps.EnsureCompiled();
            nUnitTestExecutionDriver.Execute();
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            nUnitTestExecutionDriver.Include = tag;
            WhenIExecuteTheTests();
        }

        [When(@"I execute the tests with (.*)")]
        public void WhenIExecuteTheTestsWith(string unitTestProvider)
        {
            projectSteps.EnsureCompiled();

            switch (unitTestProvider)
            {
                case "NUnit":
                    nUnitTestExecutionDriver.Execute();
                    break;
                case "MsTest":
                    msTestTestExecutionDriver.Execute();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
