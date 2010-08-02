Feature: Providing meaningful errors for wrong Gherkin files
	In order to find out easily what is wrong with a Gherkin file
	As a SpecFlow user
	I want to get proper error messages when the files are parsed

Scenario: Wrongly spelled feature keyword
	Given there is a Gherkin file as
	"""
		FeaturX: wrong feature
	"""
	When I parse the file
	Then the the following errors are provided
		| line	| error		|
		| 1		| FeaturX	|

Scenario: Wrongly spelled step keyword
	Given there is a Gherkin file as
	"""
		Feature: misspelled step keyword

		Scenario: misspelled step keyword
			Given something
			WhenX something is misspelled
	"""
	When I parse the file
	Then the the following errors are provided
		| line	| error	|
		| 5		| WhenX	|

Scenario: Restart parsing after a syntax error
	Given there is a Gherkin file as
	"""
		Feature: misspelled step keyword

		Scenario: misspelled step keyword 1
			Given something
			WhenX something is misspelled
			ThenX something is also misspelled
	"""
	When I parse the file
	Then the the following errors are provided
		| line	| error	|
		| 5		| WhenX	|
		| 6		| ThenX	|

Scenario: Table cell count mismatch
	Given there is a Gherkin file as
	"""
		Feature: Table cell count mismatch

		Scenario: Table cell count mismatch
			Given a table
				| h1 | h2 |
				| c1 | c2 | c3 |
	"""
	When I parse the file
	Then the the following errors are provided
		| line	| error																			|
		| 6		| Number of cells in the row does not match the number of cells in the header!	|

Scenario: Restart parsing after a semantic error
	Given there is a Gherkin file as
	"""
		Feature: Table cell count mismatch

		Scenario: Table cell count mismatch
			Given a table
				| h1 | h2 |
				| c1 | c2 | c3 |
				| c1 | c2 | c3 |
	"""
	When I parse the file
	Then the the following errors are provided
		| line	| error																			|
		| 6		| Number of cells in the row does not match the number of cells in the header!	|
		| 7		| Number of cells in the row does not match the number of cells in the header!	|

