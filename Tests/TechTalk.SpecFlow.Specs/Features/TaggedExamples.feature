Feature: Tagging Scenario Outline Examples
	As a developer
	I would like to be able to tag examples of the scenario outline
	So that I can write bindings that behave differently for the variants

Scenario: Examples can be tagged
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Scenario Outline with Tagged Examples
				When I do <what>

			Examples: 
				| what           |
				| something      |
				| somethign else |

			@tag1
			Examples: 
				| what                |
				| somethign different |
		"""
	And all steps are bound and pass
#this is a bug, the tagged examples can only be filtered with NUnit, if row testing is disabled
	And row testing is disabled
	When I execute the tests tagged with '@tag1'
	Then the execution summary should contain
		| Total |
		| 1     |

Scenario: Examples can be ignored
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Scenario Outline with Tagged Examples
				When I do <what>

			Examples: 
				| what           |
				| something      |
				| somethign else |

			@ignore
			Examples: 
				| what                |
				| somethign different |
		"""
	And all steps are bound and pass
	When I execute the tests
	Then the execution summary should contain
		| Total | Succeeded | Ignored |
		| 3     | 2         | 1       |
