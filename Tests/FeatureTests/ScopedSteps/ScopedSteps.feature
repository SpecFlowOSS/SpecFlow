Feature: Scoping step definitions
	As a developer
	I would like to be able to scope the step definitions (bindings) on method and class level
	So that I can implement test logic differently depending on the usage context

Attribute usage:

[StepContext(Tag = "mytag", Feature = "feature title", Scenario = "scenario title")] 

Future ideas:
* scope for previous steps ([StepContext(Step = "my previous step for doing something")])
* use regex in scopes ([StepContext(Scenario = "(my )?scenario title")])

#scoping criteria
@mytag
Scenario: Scoping step definitions to tags
	Attribute usage: [StepContext(Tag = "mytag")] 

	Given I have a step definition that is scoped to tag 'mytag'
	And I have a step definition that is scoped to tag 'othertag'
	When I execute a scenario with a tag 'mytag'
	Then the step definition 'Given I have a step definition that is scoped to tag 'mytag'' should be executed
	Then the step definition 'Given I have a step definition that is scoped to tag 'othertag'' should not be executed
	And the scenario should be executed successfully

Scenario: Scoping step definitions to features
	Attribute usage: [StepContext(Feature = "feature title")]

	Given I have a step definition that is scoped to feature 'Scoping step definitions'
	And I have a step definition that is scoped to feature 'Other feature'
	When I execute a scenario in feature 'Scoping step definitions'
	Then the step definition 'Given I have a step definition that is scoped to feature 'Scoping step definitions'' should be executed
	Then the step definition 'Given I have a step definition that is scoped to feature 'Other feature'' should not be executed
	And the scenario should be executed successfully

Scenario: Scoping step definitions to scenarios
	Attribute ugase: [StepContext(Scenario = "scenario title")] 

	Given I have a step definition that is scoped to scenario 'Scoping step definitions to scenarios'
	And I have a step definition that is scoped to scenario 'Other scenario'
	When I execute a scenario in scenario 'Scoping step definitions to scenarios'
	Then the step definition 'Given I have a step definition that is scoped to scenario 'Scoping step definitions to scenarios'' should be executed
	Then the step definition 'Given I have a step definition that is scoped to scenario 'Other scenario'' should not be executed
	And the scenario should be executed successfully

#scope combination criteria
@mytag
Scenario: Scopes can be conbined with AND
	Attribute usage: [StepContext(Feature = "feature title", Tag = "mytag")]

	Given I have a step definition that is scoped to both tag 'mytag' and feature 'Scoping step definitions'
	When I execute a scenario with a tag 'mytag' in feature 'Scoping step definitions'
	Then the step definition 'Given I have a step definition that is scoped to both tag 'mytag' and feature 'Scoping step definitions'' should be executed
	And the scenario should be executed successfully

@mytag
Scenario: Scopes can be conbined with OR
	Attribute usage: [StepContext(Feature = "feature title")][StepContext(Tag = "mytag")]

	Given I have a step definition that has two scope declaration: tag 'mytag' and feature 'Other feature'
	When I execute a scenario with a tag 'mytag' in feature 'Scoping step definitions'
	Then the step definition 'Given I have a step definition that has two scope declaration: tag 'mytag' and feature 'Other feature'' should be executed
	And the scenario should be executed successfully

#precedency criteria
@mytag
Scenario: Scoped matches have higher precedency
	Scoped matches are "stronger" than non-scoped matches (no ambiguouity).

	Given I have a step definition that is scoped to tag 'mytag'
	And I have a step definition without scope
	When I execute a scenario with a tag 'mytag'
	Then the step definition 'Given I have a step definition that is scoped to tag 'mytag'' should be executed
	Then the step definition 'I have a step definition without scope' should not be executed
	And the scenario should be executed successfully

@mytag
Scenario: Scoping step definitions of a binding class
	The attribute can be also placed on the binding class.

	Given I have a step definition that is in a class scoped to tag 'mytag'
	When I execute a scenario with a tag 'mytag'
	Then the step definition 'Given I have a step definition that is in a class scoped to tag 'mytag'' should be executed
	And the scenario should be executed successfully

@mytag
Scenario: No ambiguouity if the same method matches with multiple scopes
	Given I have a step definition that has two scope declaration: tag 'mytag' and feature 'Scoping step definitions'
	When I execute a scenario with a tag 'mytag' in feature 'Scoping step definitions'
	Then the step definition 'Given I have a step definition that has two scope declaration: tag 'mytag' and feature 'Scoping step definitions'' should be executed
	And the scenario should be executed successfully

@mytag
Scenario: More scope matches have higher precedency
	More scope matches (e.g. tag + feature) are "stronger" than less scope matches (e.g. tag) (no ambiguouity)

	Given I have a step definition that is scoped to tag 'mytag' and to scenario 'More scope matches have higher precedency'
	And I have a step definition that is scoped to tag 'mytag'
	When I execute a scenario with a tag 'mytag' in feature 'Scoping step definitions'
	Then the step definition 'Given I have a step definition that is scoped to tag 'mytag' and to scenario 'More scope matches have higher precedency'' should be executed
	Then the step definition 'Given I have a step definition that is scoped to tag 'mytag'' should not be executed
	And the scenario should be executed successfully
