Feature: Execute hooks
	As a developer
	I would like to be able to hook into pre and post conditions in SpecFlow
	So that I can set up and teardown my scenario accordingly

Scenario Outline: Should execute SpecFlow events
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And a hook 'HookFor<event>' for '<event>'
	And all steps are bound and pass
	When I execute the tests
	Then the hook 'HookFor<event>' is executed once

Examples: 
	| event               |
	| BeforeScenario      |
	| AfterScenario       |
	| BeforeFeature       |
	| AfterFeature        |
	| BeforeStep          |
	| AfterStep           |
	| BeforeScenarioBlock |
	| AfterScenarioBlock  |
	| BeforeTestRun       |
#bug: nunit does not run the AfterTestRun event
#	| AfterTestRun        |
