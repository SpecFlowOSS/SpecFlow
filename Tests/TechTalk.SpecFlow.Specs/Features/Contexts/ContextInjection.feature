Feature: Injecting context into binding classes
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
