@SingleTestConfiguration
Feature: Providing meaningful errors for wrong Gherkin files
	In order to find out easily what is wrong with a Gherkin file
	As a SpecFlow user
	I want to get proper error messages when the files are parsed

Scenario: Wrongly spelled feature keyword
	Given there is a Gherkin file as
	"""
		FeaturX: wrong feature
	"""
	When the file is parsed
	Then the following errors are provided
		| line | error                        |
		| 1    | got 'FeaturX: wrong feature' |

Scenario: Wrongly spelled step keyword
	Given there is a Gherkin file as
	"""
		Feature: misspelled step keyword

		Scenario: misspelled step keyword
			Given something
			WhenX something is misspelled
	"""
	When the file is parsed
	Then the following errors are provided
		| line | error                               |
		| 5    | got 'WhenX something is misspelled' |

Scenario: Wrongly spelled scenario outline
	Given there is a Gherkin file as
	"""
		Feature: Wrongly spelled scenario outline

		@dummy_tag
		Scenario OutlinX: Wrongly spelled scenario outline
			Given something

		Examples:
			| something |
	"""
	When the file is parsed
	Then the following errors are provided
		| line | error                                                    |
		| 4    | got 'Scenario OutlinX: Wrongly spelled scenario outline' |

Scenario: Table cell count mismatch
	Given there is a Gherkin file as
	"""
		Feature: Table cell count mismatch

		Scenario: Table cell count mismatch
			Given a table
				| h1 | h2 |
				| c1 | c2 | c3 |
	"""
	When the file is parsed
	Then the following errors are provided
		| line | error                                    |
		| 6    | inconsistent cell count within the table |

Scenario: Scenario outline with empty Examples
	Given there is a Gherkin file as
	"""
		Feature: Delayed semantic error

		Scenario Outline: Scenario outline with empty examples
			Given something
        
        Examples: 

		Scenario: proper scenario
			Given something
	"""
	When the file is parsed
	Then the following errors are provided
        | line | error                                                                           |
        | 3    | Scenario Outline 'Scenario outline with empty examples' has no examples defined |

Scenario: Scenario outline with empty Examples with a header
	Given there is a Gherkin file as
	"""
		Feature: Delayed semantic error

		Scenario Outline: Scenario outline with empty defined examples
			Given something
        
        Examples: 
            | Column |

		Scenario: proper scenario
			Given something
	"""
	When the file is parsed
	Then the following errors are provided
        | line | error                                                                                   |
        | 3    | Scenario Outline 'Scenario outline with empty defined examples' has no examples defined |

Scenario: Language not supported
	Given there is a Gherkin file as
	"""
		#language: invalid-lang
		Feature: Invalid language
	"""
	When the file is parsed
	Then the following errors are provided
		| line | error                           |
		| 1    | Language not supported: invalid |

Scenario: Duplicated scenario name
	Given there is a Gherkin file as
	"""
		Feature: Duplicated scenario name

		Scenario: Duplicated scenario
			Given something

		Scenario: Duplicated scenario 
			Given something
	"""
	When the file is parsed
	Then the following errors are provided
		| line	| error																		|
		| 6		| Feature file already contains a scenario with name 'Duplicated scenario'	|

Scenario: Duplicated example set name
	Given there is a Gherkin file as
	"""
		Feature: Duplicated example set name

		Scenario Outline: Scenario outline
			Given <something>

		Examples: duplicated example set
			| something |

		Examples: duplicated example set
			| something |
	"""
	When the file is parsed
	Then the following errors are provided
 		| line | error                                                                                              |
 		| 9    | Scenario Outline 'Scenario outline' already contains an example with name 'duplicated example set' |

Scenario: Duplicated background
	Given there is a Gherkin file as
	"""
		Feature: Duplicated background

		Background: 
			Given something

		Background: 
			Given something else
	"""
	When the file is parsed
	Then the following errors are provided
		| line | error             |
		| 6    | got 'Background:' |

