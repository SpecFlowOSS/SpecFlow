@mstest
Feature: MsTest unit test provider

Scenario Outline: Should be able to execute scenarios with basic results
	Given there is a SpecFlow project
	And the project is configured to use the MsTest provider
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and <step definition status>
	When I execute the tests with MsTest
	Then the execution summary should contain
		| Total | <result> |
		| 1     | 1        |

Examples: 
	| result    | step definition status |
	| Succeeded | pass                   |
	| Failed    | fail                   |

Scenario: Should handle scenario outlines
	Given there is a SpecFlow project
	And the project is configured to use the MsTest provider
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Simple Scenario Outline
				Given there is something
				When I do <what>
				Then something should happen
			Examples: 
				| what           |
				| something      |
				| somethign else |
		"""
	And all steps are bound and pass
	When I execute the tests with MsTest
	Then the execution summary should contain
		| Succeeded |
		| 2         |

@config
Scenario: Should be able to specify MsTest provider in the configuration
	Given there is a SpecFlow project
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and pass
	And the specflow configuration is
		"""
		<specFlow>
			<unitTestProvider name="MsTest"/>
		</specFlow>
		"""
	When I execute the tests with MsTest
	Then the execution summary should contain
		| Total | 
		| 1     | 
	