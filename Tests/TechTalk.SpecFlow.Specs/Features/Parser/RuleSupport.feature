@gherkin6
Feature: Rule Support

The (optional) Rule keyword has been added in Gherkin v6.  
A Rule block is used to group together several scenarios that belong to this 
business rule. The scenarios before the first Rule keyword so not belong to any rule.
See more: https://cucumber.io/docs/gherkin/reference/#rule

Scenario: Should be able to execute a simple passing scenario
    Given there is a feature file in the project as
        """
            Feature: Simple Feature
            Scenario: Scenario without a rule
                When I do something

			Rule: First rule
            Scenario: Scenario for the first rule
                When I do something

			Rule: Second rule
            Scenario: Scenario for the second rule
                When I do something
            Scenario Outline: Scenario Outline for the second rule
                When I do <what>
			Examples:
				| what           |
				| something      |
				| something else |
        """
    Given all steps are bound and pass
    When I execute the tests
    Then the execution summary should contain
        | Total | 
        | 5     | 