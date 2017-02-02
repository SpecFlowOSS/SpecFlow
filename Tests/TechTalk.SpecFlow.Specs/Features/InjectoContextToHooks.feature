@wip_gn
Feature: Injecting context into hooks

Background: 
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And all steps are bound and pass

Scenario: Inject FeatureContext into a BeforeFeature hook
	Given the following hook
        """
		[BeforeFeature]
		public static void BeforeFeatureHook(FeatureContext featureContext)
		{
			if (featureContext == null)
				throw new ArgumentNullException("featureContext");
		}
        """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
