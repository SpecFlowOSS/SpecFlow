Feature: xUnit v2 Parallel Smoke Tests

Background: 
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And all test files are inluded in the project

Scenario Outline: Test files can be generated
	Given I have a '<Language>' test project
	And row testing is <row test>
	When the feature files in the project are generated
	Then no generation error is reported

Examples: 
	| Language | case           | row test |
	| C#       | Normal testing | disabled |
	| C#       | Row testing    | enabled  |
	| VB.Net   | Normal testing | disabled |
	| VB.Net   | Row testing    | enabled  |

Scenario Outline: Generated classes from test files can be compiled
	Given I have a '<Language>' test project
	And row testing is <row test>
	When the project is compiled
	Then no compilation errors are reported

Examples: 
	| Language | case           | row test |
	| C#       | Normal testing | disabled |
	| C#       | Row testing    | enabled  |
	| VB.Net   | Normal testing | disabled |
	| VB.Net   | Row testing    | enabled  |
