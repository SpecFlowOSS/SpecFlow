Feature: Feature with failing scenarios
	In order to test reporting
	As a SpecFlow developer
	I want to have a feature that has failing scenarios

@ignore
Scenario: Ignored scenario
	Given I have a precondition that is successful

Scenario: Scenario with pending steps
	Given I have a pending precondition

Scenario: Scenario with failing steps
	Given I have a precondition that is failing
