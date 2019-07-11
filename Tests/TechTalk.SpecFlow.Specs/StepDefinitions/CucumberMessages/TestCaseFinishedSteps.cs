using System;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestCaseFinishedSteps
    {
        private readonly TestSuiteInitializationDriver _testSuiteInitializationDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;
        private readonly TestCaseFinishedDriver _testCaseFinishedDriver;
        private readonly MessageValidationDriver _messageValidationDriver;
        private readonly ExecutionDriver _executionDriver;

        public TestCaseFinishedSteps(
            TestSuiteInitializationDriver testSuiteInitializationDriver,
            TestSuiteSetupDriver testSuiteSetupDriver,
            TestCaseFinishedDriver testCaseFinishedDriver,
            MessageValidationDriver messageValidationDriver,
            ExecutionDriver executionDriver)
        {
            _testSuiteInitializationDriver = testSuiteInitializationDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
            _testCaseFinishedDriver = testCaseFinishedDriver;
            _messageValidationDriver = messageValidationDriver;
            _executionDriver = executionDriver;
        }

        [When(@"the scenario is finished at '(.*)'")]
        public void WhenTheScenarioIsFinishedAt(DateTime finishTime)
        {
            _testSuiteInitializationDriver.OverrideTestCaseFinishedTime = finishTime;
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _executionDriver.ExecuteTests();
        }

        [Then(@"(.*) TestCaseFinished messages have been sent")]
        public void ThenTestCaseFinishedMessagesHaveBeenSent(int numberOfMessages)
        {
            _testCaseFinishedDriver.TestCaseFinishedMessagesShouldHaveBeenSent(numberOfMessages);
        }

        [Then(@"a TestCaseFinished message has been sent with the following attributes")]
        public void ThenATestCaseFinishedMessageHasBeenSentWithTheFollowingAttributes(Table table)
        {
            _messageValidationDriver.TestCaseFinishedMessageShouldHaveBeenSent(table);
        }

        [Then(@"a TestCaseFinished message has been sent with the following TestResult")]
        public void ThenATestCaseFinishedMessageHasBeenSentWithTheFollowingTestResult(Table table)
        {
            _messageValidationDriver.TestCaseFinishedMessageShouldHaveBeenSentWithTestResult(table);
        }
    }
}
