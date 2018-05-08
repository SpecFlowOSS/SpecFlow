Feature: ScopedHooks
	Hooks are only triggered once, also if the scope is there multiple times


Scenario: One hook is called once
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass     
    And a hook 'BeforeScenarioHook' for 'BeforeScenario' for scopes 'mytag' on the class
	And the following binding class
		"""
        using TechTalk.SpecFlow;

		[Binding]
		[Scope(Tag = "mytag")]
		public class Hooks
		{
			[BeforeScenario("mytag")]
			public void BeforeScenarioHook()
			{
				System.IO.File.AppendAllText(System.IO.Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "hooks.log"), "-> hook: HookForBeforeScenarioHook");
			}
		}
		"""
	When I execute the tests
	Then the hook 'BeforeScenarioHook' is executed once
	

Scenario: Two hooks for the same event are called once each
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass
	And the following binding class
		"""                  
        using TechTalk.SpecFlow;

		[Binding]
		[Scope(Tag = "mytag")]
		public class Hooks
		{
			[BeforeScenario("mytag")]
			public void BeforeScenarioHook()
			{
				System.IO.File.AppendAllText(System.IO.Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "hooks.log"), "-> hook: Hook1");
			}

			[BeforeScenario("mytag")]
			public void BeforeScenarioHook_TheOtherOne()
			{
				System.IO.File.AppendAllText(System.IO.Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "hooks.log"), "-> hook: Hook2");
			}
		}
		"""
	When I execute the tests
	Then the hook 'Hook1' is executed once
	And the hook 'Hook2' is executed once
	

Scenario: Two hooks for diffenrent events are called once each
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass       
    And a hook 'Hook1' for 'BeforeScenario' for tags 'mytag'
    And a hook 'Hook2' for 'AfterScenario' for tags 'mytag'
	When I execute the tests
	Then the hook 'Hook1' is executed once
	And the hook 'Hook2' is executed once
	
Scenario: Two hooks for the same event with same name but in different classes are called once each
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass   
    And a hook 'Hook1' for 'BeforeScenario' for tags 'mytag'
    And a hook 'Hook2' for 'BeforeScenario' for tags 'mytag'
	When I execute the tests
	Then the hook 'Hook1' is executed once
	And the hook 'Hook2' is executed once
	

Scenario: One hook scoped on HookAttribute with two tags are executed once
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag @mySecondTag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass
    And a hook 'BeforeScenarioHook' for 'BeforeScenario' for tags 'mytag, mySecondTag'
	When I execute the tests
	Then the hook 'BeforeScenarioHook' is executed once
	

Scenario: One hook with two tags and [Scope] scoping are executed once
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag @mySecondTag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass     
    And a hook 'BeforeScenarioHook' for 'BeforeScenario' for scopes 'mytag, mySecondTag' on the hook method
	When I execute the tests
	Then the hook 'Hook1' is executed once
	