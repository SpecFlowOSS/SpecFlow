Feature: Scenario outlines

Scenario Outline: Should handle scenario outlines
	Given there is a SpecFlow project
	And row testing is <row test>
	And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Simple Scenario Outline
				Given there is something
				When I do <what>
				Then something should happen
			Examples: 
				| what           |
				| something      |
				| something else |
		"""
	And all steps are bound and pass
	When I execute the tests
	Then the execution summary should contain
		| Succeeded |
		| 2         |

Examples: 
	| case           | row test |
	| Normal testing | disabled |
	| Row testing    | enabled  |

@gherkin6
Scenario: Should support defining scenario outlines with the Scenario keyword
	A scenario with an "Examples" block should be treated as a Scenario Outline according to Gherkin v6
	See https://github.com/cucumber/cucumber/blob/master/gherkin/CHANGELOG.md#6013---2018-09-25
	Given there is a SpecFlow project
	And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario Outline
				Given there is something
				When I do <what>
				Then something should happen
			Examples: 
				| what           |
				| something      |
				| something else |
		"""
	And all steps are bound and pass
	When I execute the tests
	Then the execution summary should contain
		| Succeeded |
		| 2         |
