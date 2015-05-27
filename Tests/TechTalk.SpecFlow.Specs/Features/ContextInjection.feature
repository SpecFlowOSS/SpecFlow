Feature: Injecting context into step specifications
	As a developer
	I would like to have the system automatically inject an instance of any class as defined in the constructor of a step file
	So that I don't have to rely on the global shared state and can define the contexts required for each scenario.

Background: 
	Given the following binding class
        """
		public class SingleContext
		{
			public static int InstanceCount = 0;
			public string ScenarioTitle;

			public SingleContext()
			{
				ScenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
				InstanceCount++;
			}
		}
		public class OtherSingleContext
		{
		}
		public class NestedContext
		{
			public readonly SingleContext SingleContext;

			public NestedContext(SingleContext singleContext)
			{
				if (singleContext == null) throw new ArgumentNullException("singleContext");
				this.SingleContext = singleContext;
			}
		}
		public class DisposableContext : IDisposable
		{
			public static bool WasDisposed = false;

			public void Dispose()
			{
				WasDisposed = true;
			}
		}
        """
	And the following step definition
        """
		[Then(@"the instance count of SingleContext should be (\d+)")]
		public void ThenTheInstanceCountShouldBe(int expectedCount)
		{
			if (SingleContext.InstanceCount != expectedCount) throw new Exception("Instance count should be " + expectedCount + " but was " + SingleContext.InstanceCount);
		}
        """

Scenario: Binding class can depend on a single context
	Given the following binding class
        """
		[Binding]
		public class StepsWithSingleContext
		{
			private readonly SingleContext singleContext;

			public StepsWithSingleContext(SingleContext singleContext)
			{
				if (singleContext == null) throw new ArgumentNullException("singleContext");
				this.singleContext = singleContext;
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
		 Then the instance count of SingleContext should be 1
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Binding class can depend on multiple contexts
	Given the following binding class
        """
		[Binding]
		public class StepsWithMultipleContexts
		{
			public StepsWithMultipleContexts(SingleContext singleContext, OtherSingleContext otherContext)
			{
				if (singleContext == null) throw new ArgumentNullException("singleContext");
				if (otherContext == null) throw new ArgumentNullException("otherContext");
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
		 Then the instance count of SingleContext should be 1
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Context classes can depend on other context classes recursively
	Given the following binding class
        """
		[Binding]
		public class StepsWithNestedContext
		{
			public StepsWithNestedContext(NestedContext nestedContext, SingleContext singleContext)
			{
				if (nestedContext == null) throw new ArgumentNullException("nestedContext");
				if (singleContext == null) throw new ArgumentNullException("singleContext");
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
		 Then the instance count of SingleContext should be 1
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Context classes are shared across binding classes
	Given the following binding class
        """
		[Binding]
		public class StepsWithSingleContext
		{
			public StepsWithSingleContext(SingleContext singleContext)
			{
				if (singleContext == null) throw new ArgumentNullException("singleContext");
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
			}
		}
        """
	Given the following binding class
        """
		[Binding]
		public class OtherStepsWithSingleContext
		{
			public OtherStepsWithSingleContext(SingleContext singleContext)
			{
				if (singleContext == null) throw new ArgumentNullException("singleContext");
			}

			[When(@"I do something else")]
			public void WhenIDoSomethingElse()
			{
			}
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
         And I do something else
		 Then the instance count of SingleContext should be 1
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Context classes are recreated for every scenario
	Given the following binding class
        """
		[Binding]
		public class StepsWithSingleContext
		{
			private SingleContext singleContext;

			public StepsWithSingleContext(SingleContext singleContext)
			{
				if (singleContext == null) throw new ArgumentNullException("singleContext");
				this.singleContext = singleContext;
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
			}


			[Then(@"the SingleContext instance was created in scenario '(.+)'")]
			public void ThenTheInstanceCountShouldBe(string title)
			{
				if (singleContext.ScenarioTitle != title) throw new Exception("Instance count should be created in " + title + " but was " + singleContext.ScenarioTitle);
			}
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
		 Then the SingleContext instance was created in scenario 'Simple Scenario'
         """
	And a scenario 'Other Scenario' as
         """
         When I do something
		 Then the SingleContext instance was created in scenario 'Other Scenario'
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 2         |

Scenario: Disposable dependencies should be disposed after scenario execution
	Given the following binding class
        """
		[Binding]
		public class StepsWithSingleContext
		{
			public StepsWithSingleContext(DisposableContext context)
			{
			}

			[When(@"I do something")]
			public void WhenIDoSomething()
			{
				if (DisposableContext.WasDisposed) throw new Exception("context was disposed");
			}

			[AfterFeature]
			static public void AfterFeature()
			{
			}
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
         """
	And a scenario 'Second Scenario' as
         """
         When I do something
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded | Failed |
         | 1         | 1      |

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

