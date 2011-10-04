Feature: xUnit Smoke Tests

Background: 
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And all test files are inluded in the project

Scenario Outline: Test files can be generated
	Given row testing is <row test>
	When the feature files in the project are generated
	Then no generation error is reported

Examples: 
	| case           | row test |
	| Normal testing | disabled |
	| Row testing    | enabled  |

Scenario Outline: Generated classes from test files can be compiled
	Given row testing is <row test>
	When the project is compiled
	Then no compilation errors are reported

Examples: 
	| case           | row test |
	| Normal testing | disabled |
	| Row testing    | enabled  |

