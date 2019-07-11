using System;
using System.Linq;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestRunStartedSteps
    {
        private readonly TestRunStartedDriver _testRunStartedDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;
        private readonly TestSuiteInitializationDriver _testSuiteInitializationDriver;
        private readonly MessageValidationDriver _messageValidationDriver;
        private readonly ScenarioContext _scenarioContext;
        private readonly ExecutionDriver _executionDriver;

        public TestRunStartedSteps(
            TestRunStartedDriver testRunStartedDriver,
            TestSuiteSetupDriver testSuiteSetupDriver,
            TestSuiteInitializationDriver testSuiteInitializationDriver,
            MessageValidationDriver messageValidationDriver,
            ScenarioContext scenarioContext,
            ExecutionDriver executionDriver)
        {
            _testRunStartedDriver = testRunStartedDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
            _testSuiteInitializationDriver = testSuiteInitializationDriver;
            _messageValidationDriver = messageValidationDriver;
            _scenarioContext = scenarioContext;
            _executionDriver = executionDriver;
        }

        [When(@"the test suite is executed")]
        public void WhenTheTestSuiteIsExecuted()
        {
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _executionDriver.ExecuteTests();
        }

        [When(@"the test suite is started at '(.*)'")]
        public void WhenTheTestSuiteIsStartedAt(DateTime startTime)
        {
            _testSuiteInitializationDriver.OverrideTestSuiteStartupTime = startTime;
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _executionDriver.ExecuteTests();
        }

        [Then(@"'(\d+)' TestRunStarted messages have been sent")]
        public void ThenATestRunStartedMessageHasBeenSent(int numberOfMessages)
        {
            if (_scenarioContext.ScenarioInfo.Tags.Contains("SpecFlow"))
            {
                return;
            }

            _testRunStartedDriver.TestRunStartedMessageShouldHaveBeenSent(numberOfMessages);
        }

        [Then(@"a TestRunStarted message has been sent with the following attributes")]
        public void ThenATestRunStartedMessageHasBeenSentWithTheFollowingAttributes(Table attributesTable)
        {
            _messageValidationDriver.TestRunStartedMessageShouldHaveBeenSent(attributesTable);
        }
    }
}
