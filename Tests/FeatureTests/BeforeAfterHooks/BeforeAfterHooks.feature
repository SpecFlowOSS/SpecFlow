Feature: Addition
	As a developer
	I would like to be able to hook into pre and post conditions in SpecFlow
	So that I can set up and teardown my scenario accordingly

Scenario: Hooking into pre conditions for Test Runs in SpecFlow
	Given the scenario is running
	Then the BeforeTestRun hook should have been executed

Scenario: Hooking into pre conditions for Features in SpecFlow
	Given the scenario is running
	Then the BeforeFeature hook should have been executed

Scenario: Hooking into pre conditions for Scenarios in SpecFlow
	Given the scenario is running
	Then the BeforeScenario hook should have been executed

Scenario: Hooking into pre conditions for ScenarioBlocks in SpecFlow
	Given the scenario is running
	Then the BeforeScenarioBlock hook should have been executed

Scenario: Hooking into pre conditions for Steps in SpecFlow
	Given the scenario is running
	Then the BeforeStep hook should have been executed

