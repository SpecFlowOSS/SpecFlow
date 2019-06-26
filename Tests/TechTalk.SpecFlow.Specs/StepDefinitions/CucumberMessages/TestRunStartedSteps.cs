using System;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestRunStartedSteps
    {
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;
        private readonly TestRunStartedDriver _testRunStartedDriver;
        private readonly SolutionDriver _solutionDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;
        private readonly TestSuiteInitializationDriver _testSuiteInitializationDriver;
        private readonly MessageValidationDriver _messageValidationDriver;

        public TestRunStartedSteps(VSTestExecutionDriver vsTestExecutionDriver, TestRunStartedDriver testRunStartedDriver, SolutionDriver solutionDriver, TestSuiteSetupDriver testSuiteSetupDriver, TestSuiteInitializationDriver testSuiteInitializationDriver, MessageValidationDriver messageValidationDriver)
        {
            _vsTestExecutionDriver = vsTestExecutionDriver;
            _testRunStartedDriver = testRunStartedDriver;
            _solutionDriver = solutionDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
            _testSuiteInitializationDriver = testSuiteInitializationDriver;
            _messageValidationDriver = messageValidationDriver;
        }

        [When(@"the test suite is executed")]
        [When(@"the test suite was executed")]
        public void WhenTheTestSuiteIsExecuted()
        {
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _solutionDriver.CompileSolution(BuildTool.MSBuild);
            _solutionDriver.CheckSolutionShouldHaveCompiled();
            _vsTestExecutionDriver.ExecuteTests();
        }

        [When(@"the test suite is started at '(.*)'")]
        public void WhenTheTestSuiteIsStartedAt(DateTime startTime)
        {
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _testSuiteInitializationDriver.OverrideTestSuiteStartupTime = startTime;
        }

        [Then(@"'(\d+)' TestRunStarted messages have been sent")]
        public void ThenATestRunStartedMessageHasBeenSent(int numberOfMessages)
        {
            _testRunStartedDriver.TestRunStartedMessageShouldHaveBeenSent(numberOfMessages);
        }

        [Then(@"a TestRunStarted message has been sent with the following attributes")]
        public void ThenATestRunStartedMessageHasBeenSentWithTheFollowingAttributes(Table attributesTable)
        {
            _messageValidationDriver.TestRunStartedMessageShouldHaveBeenSent(attributesTable);
        }
    }
}
