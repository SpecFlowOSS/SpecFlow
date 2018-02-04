@wip_gn
Feature: Injecting context into hooks

Background: 
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And all steps are bound and pass



Scenario Outline: Inject FeatureContext into a Feature hook
	Given the following hook
        """
		[<Hook>]
		public static void <Hook>Hook(FeatureContext featureContext)
		{
			if (featureContext == null)
				throw new ArgumentNullException("featureContext");

			Console.WriteLine("<Hook>Hook");
		}
        """
	When I execute the tests
	Then all tests should pass
	And the execution log should contain text '<Hook>Hook'

Examples: 
	| Hook          |
	| BeforeFeature |
	| AfterFeature  |


Scenario Outline: Inject ScenarioContext into a Scenario hook
	Given the following hook
        """
		[<Hook>]
		public void <Hook>Hook(ScenarioContext scenarioContext)
		{
			if (scenarioContext == null)
				throw new ArgumentNullException("scenarioContext");

			Console.WriteLine("<Hook>Hook");
		}
        """
	When I execute the tests
	Then all tests should pass
	And the execution log should contain text '<Hook>Hook'

Examples: 
	| Hook           |
	| BeforeScenario |
	| AfterScenario  |