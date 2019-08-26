using System.Linq;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestRunFinishedSteps
    {
        private readonly MessageValidationDriver _messageValidationDriver;
        private readonly ScenarioContext _scenarioContext;
        private readonly TestRunFinishedDriver _testRunFinishedDriver;

        public TestRunFinishedSteps(MessageValidationDriver messageValidationDriver, ScenarioContext scenarioContext, TestRunFinishedDriver testRunFinishedDriver)
        {
            _messageValidationDriver = messageValidationDriver;
            _scenarioContext = scenarioContext;
            _testRunFinishedDriver = testRunFinishedDriver;
        }

        [Then(@"a TestRunFinished message has been sent with the following attributes")]
        public void ThenATestRunFinishedMessageHasBeenSentWithTheFollowingAttributes(Table attributesTable)
        {
            _messageValidationDriver.TestRunFinishedMessageShouldHaveBeenSent(attributesTable);
        }


        [Then(@"'(.*)' TestRunFinished messages have been sent")]
        public void ThenTestRunFinishedMessagesHaveBeenSent(int numberOfMessages)
        {
            if (_scenarioContext.ScenarioInfo.Tags.Contains("SpecFlow"))
            {
                return;
            }

            _testRunFinishedDriver.TestRunFinishedMessageShouldHaveBeenSent(numberOfMessages);
        }

    }
}