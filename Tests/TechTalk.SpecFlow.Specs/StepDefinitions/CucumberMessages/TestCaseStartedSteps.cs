using System;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestCaseStartedSteps
    {
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;
        private readonly TestCaseStartedDriver _testCaseStartedDriver;
        private readonly SolutionDriver _solutionDriver;
        private readonly TestSuiteInitializationDriver _testSuiteInitializationDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;
        private readonly AssertionsDriver _assertionsDriver;

        public TestCaseStartedSteps(VSTestExecutionDriver vsTestExecutionDriver, TestCaseStartedDriver testCaseStartedDriver, SolutionDriver solutionDriver, TestSuiteInitializationDriver testSuiteInitializationDriver, TestSuiteSetupDriver testSuiteSetupDriver, AssertionsDriver assertionsDriver)
        {
            _vsTestExecutionDriver = vsTestExecutionDriver;
            _testCaseStartedDriver = testCaseStartedDriver;
            _solutionDriver = solutionDriver;
            _testSuiteInitializationDriver = testSuiteInitializationDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
            _assertionsDriver = assertionsDriver;
        }

        [When(@"the scenario is executed")]
        public void WhenTheScenarioIsExecuted()
        {
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _solutionDriver.CompileSolution(BuildTool.MSBuild);
            _solutionDriver.CheckSolutionShouldHaveCompiled();
            _vsTestExecutionDriver.ExecuteTests();
        }

        [When(@"the scenario is started at '(.*)'")]
        public void WhenTheScenarioIsStartedAt(DateTime startTime)
        {
            _testSuiteInitializationDriver.OverrideTestCaseStartedTime = startTime;
            _testSuiteSetupDriver.EnsureAProjectIsCreated();
            _solutionDriver.CompileSolution(BuildTool.MSBuild);
            _solutionDriver.CheckSolutionShouldHaveCompiled();
            _vsTestExecutionDriver.ExecuteTests();
        }

        [Then(@"'(\d+)' TestCaseStarted messages have been sent")]
        public void ThenTestCaseStartedMessagesHaveBeenSent(int numberOfTestCaseStartedMessages)
        {
            _testCaseStartedDriver.TestCaseStartedMessagesShouldHaveBeenSent(numberOfTestCaseStartedMessages);
        }

        [Then(@"a TestCaseStarted message has been sent with the following attributes")]
        public void ThenATestCaseStartedMessageHasBeenSentWithTheFollowingAttributes(Table table)
        {
            _assertionsDriver.TestCaseStartedMessageShouldHaveBeenSent(table);
        }
    }
}
