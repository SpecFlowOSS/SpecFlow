@parser
Feature: Handling errors of Gherkin files
	In order to find out easily what is wrong with a Gherkin file
	As a SpecFlow user
	I want to get error messages for all possible errors

Scenario: Capturing syntax error
	Given there is a Gherkin file as
	"""
		Feature: Syntax error

		Scenario: misspelled step keyword
			Given something
			WhenX something is misspelled
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error						|
		| 5		| Parsing error near 'WhenX	|

Scenario: Capturing semantic error
	Given there is a Gherkin file as
	"""
		Feature: Semantic error

		Scenario: Table cell count mismatch
			Given a table
				| h1 | h2 |
				| c1 | c2 | c3 |
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error																			|
		| 6		| Number of cells in the row does not match the number of cells in the header	|

Scenario: Capturing delayed semantic error
	Given there is a Gherkin file as
	"""
		Feature: Delayed semantic error

		Scenario Outline: Scenario outline without examples
			Given something

		Scenario: proper scenario
			Given something
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error														|
		| 3		| There are no examples defined for the scenario outline	|


Scenario: Restart parsing after a syntax error
	Given there is a Gherkin file as
	"""
		Feature: misspelled step keyword

		Scenario: misspelled step keyword 1
			Given something
			WhenX something is misspelled
			ThenX something is also misspelled
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error						|
		| 5		| Parsing error near 'WhenX	|
		| 6		| Parsing error near 'ThenX	|

Scenario: Restart parsing after a semantic error
	Given there is a Gherkin file as
	"""
		Feature: Table cell count mismatch

		Scenario: Table cell count mismatch
			Given a table
				| h1 | h2 |
				| c1 | c2 | c3 |
			WhenX something is misspelled
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error																			|
		| 6		| Number of cells in the row does not match the number of cells in the header	|
		| 7		| Parsing error near 'WhenX														|

Scenario: Do not restart parsing after a delayed semantic error
	Given there is a Gherkin file as
	"""
		Feature: Table cell count mismatch

		Scenario Outline: Scenario outline without examples
			Given something

		Scenario Outline: Other scenario outline without examples
			Given something

		Scenario: proper scenario
			Given something
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error														|
		| 3		| There are no examples defined for the scenario outline	|

