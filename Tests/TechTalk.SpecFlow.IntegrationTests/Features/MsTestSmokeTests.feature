Feature: MsTest 2010 Smoke Tests

Background: 
	Given there is a SpecFlow project
	And the project is configured to use the MsTest.2010 provider
	And all test files are inluded in the project

Scenario: Test files can be generated
	When the feature files in the project are generated
	Then no generation error is reported

@mstest
Scenario: Generated classes from test files can be compiled
	When the project is compiled
	Then no compilation errors are reported

