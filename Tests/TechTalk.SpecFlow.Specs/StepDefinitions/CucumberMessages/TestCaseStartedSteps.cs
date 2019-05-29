using System;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestCaseStartedSteps
    {
        [When(@"the scenario is executed")]
        public void WhenTheScenarioIsExecuted()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the scenario is started at '(.*)'")]
        public void WhenTheScenarioIsStartedAt(DateTime startTime)
        {
            ScenarioContext.Current.Pending();
        }


        [Then(@"(\d+) TestCaseStarted messages have been sent")]
        public void ThenTestCaseStartedMessagesHaveBeenSent(int numberOfTestCaseStartedMessages)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"a TestCaseStarted message has been sent with the following attributes")]
        public void ThenATestCaseStartedMessageHasBeenSentWithTheFollowingAttributes(Table table)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
