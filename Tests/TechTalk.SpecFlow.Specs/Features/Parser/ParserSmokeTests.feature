@SingleTestConfiguration
Feature: Parser smoke tests

Scenario Outline: Test files can be parsed
	When the test file '<test file>.feature' is parsed
	Then no parsing error is reported

Examples: 
	| test file                |
	| and                      |
	| asterisks                |
	| background               |
	| background_withtitle     |
	| but                      |
	| comments                 |
	| dutch                    |
	| featureheader            |
	| french                   |
	| full                     |
	| german                   |
	| givenwhenthenduplication |
	| hungarian                |
	| mixedgivenwhenthen       |
	| multilineargument        |
	| multilinetitle           |
	| scenariooutline          |
	| simple                   |
	| swedish                  |
	| tableargument            |
	| taggedexamples           |
	| tags                     |
	| whitespaces              |


Scenario Outline: Parsed test files produce the same result as before
	When the test file '<test file>.feature' is parsed
	#DEV: uncomment the next line and comment the Then step to overwrite the saved results
	#And the parsed result is saved to '<test file>.feature.json'
	Then the parsed result is the same as '<test file>.feature.json'

Examples: 
	| test file                |
	| and                      |
	| asterisks                |
	| background               |
	| background_withtitle     |
	| but                      |
	| comments                 |
	| dutch                    |
	| featureheader            |
	| french                   |
	| full                     |
	| german                   |
	| givenwhenthenduplication |
	| hungarian                |
	| mixedgivenwhenthen       |
	| multilineargument        |
	| multilinetitle           |
	| scenariooutline          |
	| simple                   |
	| swedish                  |
	| tableargument            |
	| taggedexamples           |
	| tags                     |
	| whitespaces              |


