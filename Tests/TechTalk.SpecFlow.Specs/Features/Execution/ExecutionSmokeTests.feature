Feature: Execution Smoke Tests

Scenario: Scenarios in the test file can be executed
	Given there is a SpecFlow project
	And all test files are inluded in the project
	And all steps are bound and pass
	When I execute the tests
	Then the execution summary should contain
		| Failed |
		| 0      |
	

Scenario Outline: Test files can be generated
	Given I have a '<Language>' test project
	And all test files are inluded in the project
	When the feature files in the project are generated
	Then no generation error is reported

Examples: 
	| Language |
	| C#       |
	| VB.Net   |

Scenario Outline: Generated classes from test files can be compiled
	Given I have a '<Language>' test project
	And all test files are inluded in the project
	When I compile the solution
	Then no compilation errors are reported

Examples: 
	| Language |
	| C#       |
	| VB.Net   |