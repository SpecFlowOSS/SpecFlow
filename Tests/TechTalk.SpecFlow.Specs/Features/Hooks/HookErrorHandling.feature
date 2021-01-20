Feature: Hook Error Handling


Scenario: Subsequent BeforeScenario hooks are not executed after an error
	Given there is a feature file in the project as
         """
		Feature: Simple Feature

		@mytag
		Scenario: Simple Scenario
		When I do something
         """
	And all steps are bound and pass
	And a hook 'Hook1' for 'BeforeScenario' with order '1000'
	And a hook 'Hook2' for 'BeforeScenario' with order '2000' throwing an exception
	And a hook 'Hook3' for 'BeforeScenario' with order '3000'
	When I execute the tests
	Then the hook 'Hook1' is executed once
	Then the hook 'Hook2' is executed 0 times
	Then the hook 'Hook3' is executed 0 times

Scenario: Should set the TestExecution status to TestError when a BeforeScenario hook throw
	Given there is a feature file in the project as
        """
		Feature: Simple Feature

		@mytag
		Scenario: Simple Scenario
		When I do something
         """
	And all steps are bound and pass
	And a hook 'Hook1' for 'BeforeScenario' with order '1000' throwing an exception
	And the following binding class
		"""
		using System;
       
		[Binding]
		public class ScenarioExecutionStatusSteps
		{
			[AfterScenario]
			public void LogScenarioExecutionStatus(ScenarioContext scenarioContext)
			{
			global::Log.LogHook("ScenarioExecutionStatus:" + scenarioContext.ScenarioExecutionStatus);            
			global::Log.LogHook("TestError:" + scenarioContext?.TestError?.Message);            
			}
		}
	   """
	When I execute the tests
	Then the log file '..\steps.log' should contain text 'ScenarioExecutionStatus:TestError'
	And the log file '..\steps.log' should contain text 'TestError:Error in Hook: BeforeScenario'

Scenario: Steps should be executed as skipped after a BeforeScenario hook throws
Given there is a feature file in the project as
        """
		Feature: Simple Feature

		@mytag
		Scenario: Simple Scenario
		When I do something
         """
	And all steps are bound and pass
	And a hook 'Hook1' for 'BeforeScenario' with order '1000' throwing an exception	
	When I execute the tests
	Then the execution log should contain text
	"""
	When I do something
	-> skipped because of previous errors
	"""

Scenario: Subsequent AfterScenario hooks are not executed after an error
	Given there is a feature file in the project as
         """
		Feature: Simple Feature

		@mytag
		Scenario: Simple Scenario
		When I do something
         """
	And all steps are bound and pass
	And a hook 'Hook1' for 'AfterScenario' with order '1000'
	And a hook 'Hook2' for 'AfterScenario' with order '2000' throwing an exception
	And a hook 'Hook3' for 'AfterScenario' with order '3000'
	When I execute the tests
	Then the hook 'Hook1' is executed once
	Then the hook 'Hook2' is executed 0 times
	Then the hook 'Hook3' is executed 0 times