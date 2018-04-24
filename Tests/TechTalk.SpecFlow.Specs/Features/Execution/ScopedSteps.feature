Feature: Scoping step definitions
	As a developer
	I would like to be able to scope the step definitions (bindings) on method and class level
	So that I can implement test logic differently depending on the usage context

Attribute usage:

[Scope(Tag = "mytag", Feature = "feature title", Scenario = "scenario title")] 

Future ideas:
* scope for previous steps ([StepContext(Step = "my previous step for doing something")])
* use regex in scopes ([StepContext(Scenario = "(my )?scenario title")])

Scenario: Scoping step definitions to tags
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And the following step definitions
		 """
			[When("I do something"), Scope(Tag = "mytag")]
			public void WhenIDoSomethingWithMyTag()
			{}

			[When("I do something"), Scope(Tag = "othertag")]
			public void WhenIDoSomethingWithOtherTag()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingWithMyTag' is executed

Scenario: Scoping step definitions to features
	Given there is a feature file in the project as
         """
			Feature: Simple Feature
			Scenario: Simple Scenario
			When I do something
         """
	And the following step definitions
		 """
			[When("I do something"), Scope(Feature = "Simple Feature")]
			public void WhenIDoSomethingInSimpleFeature()
			{}

			[When("I do something"), Scope(Feature = "Other Feature")]
			public void WhenIDoSomethingInOtherFeature()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingInSimpleFeature' is executed


Scenario: Scoping step definitions to scenarios
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			Scenario: Simple Scenario
			When I do something
         """
	And the following step definitions
		 """
			[When("I do something"), Scope(Scenario = "Simple Scenario")]
			public void WhenIDoSomethingInSimpleScenario()
			{}

			[When("I do something"), Scope(Scenario = "Other Scenario")]
			public void WhenIDoSomethingInOtherScenario()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingInSimpleScenario' is executed

Scenario: Scopes can be conbined with AND
	Given there is a feature file in the project as
         """
			Feature: Simple Feature
			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And the following step definition
		 """
			[When("I do something"), Scope(Feature = "Simple Feature", Tag = "mytag")]
			public void WhenIDoSomethingInSimpleFeatureAndMyTag()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingInSimpleFeatureAndMyTag' is executed

Scenario: Scopes can be conbined with OR
	Given there is a feature file in the project as
         """
			Feature: Simple Feature
			@mytag
			Scenario: Simple Scenario
			When I do something

			Scenario: Other Scenario
			When I do something
         """
	And the following step definition
		 """
			[When("I do something"), Scope(Scenario = "Other Scenario"), Scope(Tag = "mytag")]
			public void WhenIDoSomethingInOtherScenarioOrMyTag()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingInOtherScenarioOrMyTag' is executed twice

Scenario: Scoped matches have higher precedency
	Scoped matches are "stronger" than non-scoped matches (no ambiguouity).
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And the following step definitions
		 """
			[When("I do something"), Scope(Tag = "mytag")]
			public void WhenIDoSomethingWithMyTag()
			{}

			[When("I do something")]
			public void WhenIDoSomethingNonScoped()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingWithMyTag' is executed

Scenario: Scoping step definitions of a binding class
	The attribute can be also placed on the binding class.
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And the following binding class
		 """
		 [Binding, Scope(Tag = "mytag")]
		 public class ScopedSteps
		 {
			[When("I do something")]
			public void WhenIDoSomethingWithMyTag()
			{}
		 }
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingWithMyTag' is executed


Scenario: No ambiguouity if the same method matches with multiple scopes
	Given there is a feature file in the project as
         """
			Feature: Simple Feature
			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And the following step definition
		 """
			[When("I do something"), Scope(Scenario = "Simple Scenario"), Scope(Tag = "mytag")]
			public void WhenIDoSomethingInOtherScenarioOrMyTag()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingInOtherScenarioOrMyTag' is executed

Scenario: More scope matches have higher precedency
	More scope matches (e.g. tag + feature) are "stronger" than less scope matches (e.g. tag) (no ambiguouity)
	Given there is a feature file in the project as
         """
			Feature: Simple Feature
			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And the following step definition
		 """
			[When("I do something"), Scope(Feature = "Simple Feature", Tag = "mytag")]
			public void WhenIDoSomethingInSimpleFeatureAndMyTag()
			{}
			[When("I do something"), Scope(Tag = "mytag")]
			public void WhenIDoSomethingWithMyTag()
			{}
		 """
	When I execute the tests
	Then the binding method 'WhenIDoSomethingInSimpleFeatureAndMyTag' is executed

