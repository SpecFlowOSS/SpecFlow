Feature: Execution Smoke Tests

Scenario: Scenarios in the test file can be executed
	Given there is a SpecFlow project
	And all test files are inluded in the project
	And all steps are bound and pass
	When I execute the tests
	Then the execution summary should contain
		| Failed |
		| 0      |
	