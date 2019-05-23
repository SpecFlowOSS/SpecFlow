using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestRunStartedSteps
    {
        [Then(@"a TestRunStarted message has been sent")]
        public void ThenATestRunStartedMessageHasBeenSent()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the test suite is executed")]
        public void WhenTheTestSuiteIsExecuted()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the test suite is started at '(.*)'")]
        public void WhenTheTestSuiteIsStartedAt(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the test suite was executed")]
        public void WhenTheTestSuiteWasExecuted()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
