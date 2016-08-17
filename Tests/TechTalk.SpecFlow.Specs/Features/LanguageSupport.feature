Feature: LanguageSupport
	


Scenario: C#
	Given I have a 'C#' test project
	And there is a feature file in the project as
		"""
		Feature: Simple Feature
		Scenario: Simple Scenario
			When I do something
		"""
	And all steps are bound and pass

	When I execute the tests

	Then the execution summary should contain
		| Total | Succeeded |
		| 1     | 1         |

Scenario: VB.Net
	Given I have a 'VB.Net' test project
	And there is a feature file in the project as
		"""
		Feature: Simple Feature
		Scenario: Simple Scenario
			When I do something
		"""
	And all steps are bound and pass

	When I execute the tests

	Then the execution summary should contain
		| Total | Succeeded |
		| 1     | 1         |
