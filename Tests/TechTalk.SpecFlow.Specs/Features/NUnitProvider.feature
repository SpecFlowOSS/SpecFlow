Feature: NUnit unit test provider

Scenario Outline: Should be able to execute scenarios with basic results
	Given there is a SpecFlow project
	And the project is configured to use the NUnit provider
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and <step definition status>
	When I execute the tests with NUnit
	Then the execution summary should contain
		| Total | <result> |
		| 1     | 1        |

Examples: 
	| result    | step definition status |
	| Succeeded | pass                   |
	| Failed    | fail                   |

Scenario Outline: Should handle scenario outlines
	Given there is a SpecFlow project
	And the project is configured to use the NUnit provider
	And row testing is <row test>
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
	When I execute the tests with NUnit
	Then the execution summary should contain
		| Succeeded |
		| 2         |

Examples: 
	| case           | row test |
	| Normal testing | disabled |
	| Row testing    | enabled  |

@config
Scenario: Should be able to specify NUnit provider in the configuration
	Given there is a SpecFlow project
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and pass
	And the specflow configuration is
		"""
		<specFlow>
			<unitTestProvider name="NUnit"/>
		</specFlow>
		"""
	When I execute the tests with NUnit
	Then the execution summary should contain
		| Total | 
		| 1     | 
	