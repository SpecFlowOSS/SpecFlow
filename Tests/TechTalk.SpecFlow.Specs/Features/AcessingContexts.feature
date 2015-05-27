Feature: Acessing Contexts

Scenario: Should be able to inject ScenarioContext
	Given the following binding class
        """
		[Binding]
		public class StepsWithScenarioContext
		{
			public StepsWithScenarioContext(ScenarioContext scenarioContext)
			{
				if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
			}
		}
        """	
	And a scenario 'Simple Scenario' as
         """
         When I do something         
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: The same ScenarioContext should be inject in the same scenario
	Given the following binding class
        """
		[Binding]
		public class StepsWithScenarioContext
		{
			private readonly ScenarioContext scenarioContext;

			public StepsWithScenarioContext(ScenarioContext scenarioContext)
			{
				if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
				this.scenarioContext = scenarioContext;
			}

			[Given(@"I put something into the context")]
			public void GivenIPutSomethingIntoTheContext()
			{
				scenarioContext.Set("test-value", "test-key");
			}
		}
        """	
	Given the following binding class
        """
		[Binding]
		public class AnotherStepsWithScenarioContext
		{
		    private readonly ScenarioContext scenarioContext;

			public AnotherStepsWithScenarioContext(ScenarioContext scenarioContext)
			{
				if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
				this.scenarioContext = scenarioContext;
			}

			[Then(@"something should be found in the context")]
			public void ThenSomethingShouldBeFoundInTheContext()
			{
				var testValue = scenarioContext.Get<string>("test-key");
				if (testValue != "test-value") throw new Exception("Test value was not found in the scenarioContext"); 
			}
		}
        """	
	And a scenario 'Simple Scenario' as
         """
		 Given I put something into the context         
		 Then something should be found in the context
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Different scenarios should have their own ScenarioContext injected
	Given the following binding class
        """
		[Binding]
		public class StepsWithScenarioContext
		{
			private readonly ScenarioContext scenarioContext;

			public StepsWithScenarioContext(ScenarioContext scenarioContext)
			{
				if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
				this.scenarioContext = scenarioContext;
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
				string testValue = null;
				if (scenarioContext.TryGetValue("test-key", out testValue)) throw new Exception("Test value was found in the scenarioContext"); 
				scenarioContext.Set("test-value", "test-key");
			}
		}
        """	
	And a scenario 'Simple Scenario' as
         """
         When I do something         
         """
	And a scenario 'Another Simple Scenario' as
         """
         When I do something         
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 2         |

Scenario: Should be able to inject FeatureContext
	Given the following binding class
        """
		[Binding]
		public class StepsWithScenarioContext
		{
			public StepsWithScenarioContext(FeatureContext featureContext)
			{
				if (featureContext == null) throw new ArgumentNullException("featureContext");
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
			}
		}
        """	
	And a scenario 'Simple Scenario' as
         """
         When I do something         
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: The same FeatureContext should be inject in the scenarios of the same feature
	Given the following binding class
        """
		[Binding]
		public class StepsWithFeatureContext
		{
			private readonly FeatureContext featureContext;

			public StepsWithFeatureContext(FeatureContext featureContext)
			{
				if (featureContext == null) throw new ArgumentNullException("featureContext");
				this.featureContext = featureContext;
			}

			[Given(@"I put something into the context")]
			public void GivenIPutSomethingIntoTheContext()
			{
				featureContext.Set("test-value", "test-key");
			}
		}
        """	
	Given the following binding class
        """
		[Binding]
		public class AnotherStepsWithFeatureContext
		{
		    private readonly FeatureContext featureContext;

			public AnotherStepsWithFeatureContext(FeatureContext featureContext)
			{
				if (featureContext == null) throw new ArgumentNullException("featureContext");
				this.featureContext = featureContext;
			}

			[Then(@"something should be found in the context")]
			public void ThenSomethingShouldBeFoundInTheContext()
			{
				var testValue = featureContext.Get<string>("test-key");
				if (testValue != "test-value") throw new Exception("Test value was not found in the scenarioContext"); 
			}
		}
        """	
	And there is a feature file in the project as
         """
		 Feature: Feature1
	
		 Scenario: Scenario1
			Given I put something into the context  

		 Scenario: Scenario2
			Then something should be found in the context
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 2         |

