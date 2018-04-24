Feature: Basic scenario execution

Background: 
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something
				When I do something
				Then something should happen
		"""

Scenario: Should be able to execute a simple passing scenario
	Given all steps are bound and pass
	When I execute the tests
	Then the execution result should contain
		| Total | TotalSucceeded |
		| 1     | 1              |


Scenario: Should be able to execute a simple failing scenario
	Given all steps are bound and fail
	When I execute the tests
	Then the execution summary should contain
		| Total | Failed |
		| 1     | 1      |

Scenario: Should be able to execute a simple pending scenario
	When I execute the tests
	Then the execution summary should contain
		| Total | Pending |
		| 1     | 1       |
