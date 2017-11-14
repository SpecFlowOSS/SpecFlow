Feature: XUnit v2 unit test provider

Scenario Outline: Should be able to execute scenarios with basic results
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and <step definition status>
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Total | <result> |
		| 1     | 1        |

Examples: 
	| result    | step definition status |
	| Succeeded | pass                   |
	| Failed    | fail                   |

	Scenario: Should be able to ignore a scenario outline
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And there is a feature file in the project as
		"""
			Feature: Simple Feature
			@ignore
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
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Succeeded |
		| 0         |

Scenario: Should be able to ignore a scenario
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And there is a feature file in the project as
		"""
			Feature: Simple Feature
			
            Scenario: First
                Given there is something
                Then something should happen

            @ignore
            Scenario: Second
                Given there is something
                Then something should happen
		"""
	And all steps are bound and pass
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Succeeded | Ignored |
		| 1         | 1       |

Scenario: Should be able to ignore all Scenarios in a feature
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And there is a feature file in the project as
		"""
            @ignore
			Feature: Simple Feature
			
            Scenario: First
                Given there is something
                Then something should happen

            @ignore
            Scenario: Second
                Given there is something
                Then something should happen

            Scenario: Third
                Given there is something else
                Then something else should happen
		"""
	And all steps are bound and pass
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Succeeded | Ignored |
		| 0         | 3       |

Scenario: Should be able to ignore all Scenarios and Scenario Outlines in a feature
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And there is a feature file in the project as
		"""
            @ignore
			Feature: Simple Feature
			
            Scenario: First
                Given there is something
                Then something should happen

            Scenario Outline: First Outline
				Given there is something
				When I do <what>
				Then something should happen
			Examples: 
				| what           |
				| something      |
				| something else |

            @ignore
            Scenario Outline: Second Outline
				Given there is something
				When I do <what>
				Then something should happen
			Examples: 
				| what           |
				| something      |
				| something else |

            Scenario: Last
                Given there is something
                Then something should happen
		"""
	And all steps are bound and pass
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Succeeded | Ignored |
		| 0         | 4       |

Scenario Outline: Should handle scenario outlines
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
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
				| something else |
		"""
	And all steps are bound and pass
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Succeeded |
		| 2         |

Examples: 
	| case           | row test |
	| Normal testing | disabled |
	| Row testing    | enabled  |

@config
Scenario: Should be able to specify xUnit provider in the configuration
	Given there is a SpecFlow project
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and pass
	And the specflow configuration is
		"""
		<specFlow>
			<unitTestProvider name="xUnit"/>
		</specFlow>
		"""
	When I execute the tests with xUnit
	Then the execution summary should contain
		| Total | 
		| 1     | 
	
Scenario: Should be able to log custom messages
Given there is a SpecFlow project
And the project is configured to use the xUnit provider
And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""	
And the following step definition
         """
         [When(@"I do something")]
		 public void WhenIDoSomething()
		 {
			ScenarioContext.Current.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>().WriteLine("hello");
		 }
         """
		 When I execute the tests with xUnit
	Then the execution summary should contain
		| Succeeded |
		| 1         |



Scenario: Should be able to log custom messages using context injection
	Given there is a SpecFlow project
	And the project is configured to use the xUnit provider
	And the following binding class
        """
		[Binding]
		public class StepsWithScenarioContext
		{
			private ScenarioContext _scenarioContext;
			private Xunit.Abstractions.ITestOutputHelper _output;
			public StepsWithScenarioContext(ScenarioContext scenarioContext)
			{
				_scenarioContext = scenarioContext;
				_output = _scenarioContext.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>();
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
				_output.WriteLine("something");
			}
		}
        """	
	And a scenario 'Simple Scenario' as
         """
         When I do something         
         """
	When I execute the tests with xUnit
	Then the execution summary should contain
         | Succeeded |
         | 1         |