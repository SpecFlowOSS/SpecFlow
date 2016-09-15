Feature: MsTest 2010 Smoke Tests

Background: 
	Given there is a SpecFlow project
	And the project is configured to use the MsTest.2010 provider
	And all test files are inluded in the project

Scenario Outline: Test files can be generated
	Given I have a '<Language>' test project
	When the feature files in the project are generated
	Then no generation error is reported

Examples: 
	| Language |
	| C#       |
	| VB.Net   |

@mstest
Scenario Outline: Generated classes from test files can be compiled
	Given I have a '<Language>' test project
	When the project is compiled
	Then no compilation errors are reported

Examples: 
	| Language |
	| C#       |
	| VB.Net   |