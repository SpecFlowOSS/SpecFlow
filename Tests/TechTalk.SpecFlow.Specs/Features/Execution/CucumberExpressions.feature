@focus
Feature: Cucumber expressions

Rule: Shoud support Cucumber expressions

Scenario: Simple Cucumber expresions are used for Step Definitions
	Given a scenario 'Simple Scenario' as
         """
			When I have 42 cucumbers in my belly
         """
	And the following step definition
        """
		[When(@"I have {int} cucumbers in my belly")]
		public void WhenIHaveCucumbersInMyBelly(int count)
		{
            global::Log.LogStep(); 
		}
        """
	When I execute the tests
	Then the binding method 'WhenIHaveCucumbersInMyBelly' is executed


Rule: Regular expressions and Cucumber expressions can be mixed

Rule: Custom parameter types can be used in Cucumber expressions