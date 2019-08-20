Feature: Execute async hooks
	As a developer
	I would like to be able to hook asynchronous code into pre and post conditions in SpecFlow
	So that I can set up and teardown my scenario accordingly

Scenario Outline: Should execute SpecFlow events
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And an async hook 'HookFor<event>' for '<event>' including locking
	And all steps are bound and pass
	And hook 'HookFor<event>' log file is locked
	And I start executing the tests asynchronously
	And tests are waiting for hook lock 'HookFor<event>'
	When hook 'HookFor<event>' file lock is released
	Then tests finish successfully
	And the hook 'HookFor<event>' is executed once

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
	| AfterTestRun        |