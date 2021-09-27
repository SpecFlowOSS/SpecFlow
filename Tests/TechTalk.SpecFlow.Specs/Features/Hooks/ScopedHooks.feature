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
    And a hook 'BeforeScenarioHook' for 'BeforeScenario' with tag 'mytag' and class scope 'mytag'
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
    And a hook 'Hook1' for 'BeforeScenario' with tag 'mytag' and class scope 'mytag'
    And a hook 'Hook2' for 'BeforeScenario' with tag 'mytag' and class scope 'mytag'
	When I execute the tests
	Then the hook 'Hook1' is executed once
	And the hook 'Hook2' is executed once
	

Scenario: Two hooks for different events are called once each
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass       
    And a hook 'Hook1' for 'BeforeScenario' with tag 'mytag'
    And a hook 'Hook2' for 'AfterScenario' with tag 'mytag'
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
    And a hook 'Hook1' for 'BeforeScenario' with tags 'mytag'
    And a hook 'Hook2' for 'BeforeScenario' with tags 'mytag'
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
    And a hook 'BeforeScenarioHook' for 'BeforeScenario' with tags 'mytag, mySecondTag'
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
    And a hook 'Hook1' for 'BeforeScenario' with method scopes 'mytag, mySecondTag'
	When I execute the tests
	Then the hook 'Hook1' is executed once
	