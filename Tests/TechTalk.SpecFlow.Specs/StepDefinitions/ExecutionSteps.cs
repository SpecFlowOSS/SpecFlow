using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly ProjectSteps projectSteps;
        private readonly SpecFlowConfigurationDriver configurationDriver;
        private readonly NUnitTestExecutionDriver nUnitTestExecutionDriver;
        private readonly XUnitTestExecutionDriver xUnitTestExecutionDriver;
        private readonly MsTestTestExecutionDriver msTestTestExecutionDriver;

        public ExecutionSteps(NUnitTestExecutionDriver nUnitTestExecutionDriver, XUnitTestExecutionDriver xUnitTestExecutionDriver,
            SpecFlowConfigurationDriver configurationDriver, MsTestTestExecutionDriver msTestTestExecutionDriver,
            ProjectSteps projectSteps)
        {
            this.nUnitTestExecutionDriver = nUnitTestExecutionDriver;
            this.xUnitTestExecutionDriver = xUnitTestExecutionDriver;
            this.projectSteps = projectSteps;
            this.msTestTestExecutionDriver = msTestTestExecutionDriver;
            this.configurationDriver = configurationDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            configurationDriver.UnitTestProviderName.Should().Be("NUnit");

            projectSteps.EnsureCompiled();
            nUnitTestExecutionDriver.ExecuteWithNUnit3();
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
                case "NUnit.2":
                    nUnitTestExecutionDriver.ExecuteWithNUnit2();
                    break;
                case "NUnit":
                    nUnitTestExecutionDriver.ExecuteWithNUnit3();
                    break;
                case "MsTest":
                    msTestTestExecutionDriver.Execute();
                    break;
                case "xUnit":
                    xUnitTestExecutionDriver.Execute();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
