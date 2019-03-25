@featureTag
Feature: SpecFlowFeature1
	The feature tag is used to scope the Step Definitnion class
	both scenario tags are scoped to a different method that uses the same [When...] definition.
	Binding of both When steps is not working.
	If the feature tag is removed, scenarios are bound properly (but all other bindings within the class are broken)
	

@scenarioTag1
Scenario: Add two numbers
	the scenario tag 1 is used to scope the "I press add" step
	it is scoped to the method WhenIPressAdd()

	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen

@scenarioTag2
Scenario: Substract two numbers
	the scenario tag 2 is used to scope the "I press add" step
	it is scoped to the method WhenIPressAddVariant()

	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
