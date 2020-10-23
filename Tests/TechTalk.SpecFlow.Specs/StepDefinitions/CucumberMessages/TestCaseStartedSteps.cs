using System;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestCaseStartedSteps
    {
        private readonly TestCaseStartedDriver _testCaseStartedDriver;
        private readonly TestSuiteInitializationDriver _testSuiteInitializationDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;
        private readonly MessageValidationDriver _messageValidationDriver;
        private readonly ExecutionDriver _executionDriver;

        public TestCaseStartedSteps(
            TestCaseStartedDriver testCaseStartedDriver,
            TestSuiteInitializationDriver testSuiteInitializationDriver,
            TestSuiteSetupDriver testSuiteSetupDriver,
            MessageValidationDriver messageValidationDriver,
            ExecutionDriver executionDriver)
        {
            _testCaseStartedDriver = testCaseStartedDriver;
            _testSuiteInitializationDriver = testSuiteInitializationDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
            _messageValidationDriver = messageValidationDriver;
            _executionDriver = executionDriver;
        }

        [When(@"the scenario is executed")]
        public void WhenTheScenarioIsExecuted()
        {
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _executionDriver.ExecuteTests();
        }

        [When(@"the scenario is started at '(.*)'")]
        public void WhenTheScenarioIsStartedAt(DateTime startTime)
        {
            _testSuiteInitializationDriver.OverrideTestCaseStartedTime = startTime;
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _executionDriver.ExecuteTests();
        }

        [Then(@"'(\d+)' TestCaseStarted messages have been sent")]
        public void ThenTestCaseStartedMessagesHaveBeenSent(int numberOfTestCaseStartedMessages)
        {
            _testCaseStartedDriver.TestCaseStartedMessagesShouldHaveBeenSent(numberOfTestCaseStartedMessages);
        }

        [Then(@"a TestCaseStarted message has been sent with the following attributes")]
        public void ThenATestCaseStartedMessageHasBeenSentWithTheFollowingAttributes(Table table)
        {
            _messageValidationDriver.TestCaseStartedMessageShouldHaveBeenSent(table);
        }

        [Then(@"a TestCaseStarted message has been sent with the following platform information")]
        public void ThenATestCaseStartedMessageHasBeenSentWithTheFollowingPlatformInformation(Table table)
        {
            _messageValidationDriver.TestCaseStartedMessageShouldHaveBeenSentWithPlatformInformation(table);
        }

        [Then(@"the TestCaseStarted message contains the following platform information attributes")]
        public void ThenTheTestCaseStartedMessageContainsTheFollowingPlatformInformationAttributes(Table attributes)
        {
            _messageValidationDriver.TestCaseStartedMessageShouldHaveBeenSentWithPlatformInformationAttributes(attributes);
        }
    }
}
