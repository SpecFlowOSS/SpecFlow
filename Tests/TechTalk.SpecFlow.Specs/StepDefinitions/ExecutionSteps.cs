using System;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly ProjectSteps projectSteps;
        private readonly AppConfigConfigurationDriver configurationDriver;
        private readonly NUnit3TestExecutionDriver nUnit3TestExecutionDriver;
        private readonly NUnit2TestExecutionDriver nUnit2TestExecutionDriver;
        private readonly XUnitTestExecutionDriver xUnitTestExecutionDriver;
        private readonly MsTestTestExecutionDriver msTestTestExecutionDriver;

        public ExecutionSteps(NUnit3TestExecutionDriver nUnit3TestExecutionDriver, NUnit2TestExecutionDriver nUnit2TestExecutionDriver, XUnitTestExecutionDriver xUnitTestExecutionDriver,
            AppConfigConfigurationDriver configurationDriver, MsTestTestExecutionDriver msTestTestExecutionDriver,
            ProjectSteps projectSteps)
        {
            this.nUnit3TestExecutionDriver = nUnit3TestExecutionDriver;
            this.nUnit2TestExecutionDriver = nUnit2TestExecutionDriver;
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
            nUnit3TestExecutionDriver.Execute();
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            nUnit3TestExecutionDriver.Include = tag;
            WhenIExecuteTheTests();
        }

        [When(@"I execute the tests with (.*)")]
        public void WhenIExecuteTheTestsWith(string unitTestProvider)
        {
            projectSteps.EnsureCompiled();

            switch (unitTestProvider)
            {
                case "NUnit.2":
                    nUnit2TestExecutionDriver.Execute();
                    break;
                case "NUnit":
                    nUnit3TestExecutionDriver.Execute();
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
