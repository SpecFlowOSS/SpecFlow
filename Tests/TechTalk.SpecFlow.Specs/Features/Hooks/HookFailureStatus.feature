Feature: HookFailureStatus
    As a developer
    I would like to know if a test failed in hooks from ScenarioExecutionStatus
    So that I can report on all errors in my cleanup steps

Background:
	Given the following binding class
		"""
		using System;
        
		[Binding]
    public class HookFailureStatusSteps
    {
        private readonly ScenarioContext scenarioContext;

        public HookFailureStatusSteps(ScenarioContext scenarioContext)
        {

            this.scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenarioHookFailure()
        {
            throw new Exception("Test failed in before scenario hook.");
        }

        [Given(@"Foo")]
        public void Foo()
        {
            //empty
        }

        [When(@"I run a test with a broken hook")]
        public void WhenIRunATestWithABrokenHook()
        {
            //empty
        }

        [Then(@"Bar")]
        public void Bar()
        {
            //empty
        }

        [AfterScenario]
        public void TheScenarioExecutionStatusShouldBeFailed()
        {
            if (ScenarioExecutionStatus.TestError != scenarioContext.ScenarioExecutionStatus)
            {
                throw new Exception($"The scenario context was expected to be \"Failed\" " +
                                    $"but was \"{scenarioContext.ScenarioExecutionStatus}\".");
            }
            else
            {
                throw new Exception("The ScenarioExecutionStatus was as expected in the AfterScenario hook.");
            }
        }
    }
		"""

Scenario: ScenarioExecutionStatus_ReportsHookFailures
	Given a scenario 'ScenarioExecutionStatus_ReportsHookFailures' as
		"""
		Given Foo
		When I run a test with a broken hook
		Then Bar
		"""
	When I execute the tests
	Then the execution log should contain text 'The ScenarioExecutionStatus was as expected in the AfterScenario hook.'
