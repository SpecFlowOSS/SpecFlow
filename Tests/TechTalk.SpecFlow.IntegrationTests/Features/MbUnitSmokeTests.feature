Feature: MbUnit v2 Smoke Tests

Background: 
	Given there is a SpecFlow project
	And the project is configured to use the MbUnit provider
	And all test files are inluded in the project

Scenario Outline: Test files can be generated
	Given row testing is <row test>
	When the feature files in the project are generated
	Then no generation error is reported

Examples: 
	| case           | row test |
	| Normal testing | disabled |
	| Row testing    | enabled  |

