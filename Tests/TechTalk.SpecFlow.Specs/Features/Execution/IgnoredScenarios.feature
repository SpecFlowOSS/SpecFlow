Feature: Ignored Scenarios

Scenario: Should be able to ignore a scenario outline
    Given there is a SpecFlow project
    And there is a feature file in the project as
        """
            Feature: Simple Feature
            @ignore
            Scenario Outline: Simple Scenario Outline
                Given there is something
                When I do <what>
                Then something should happen
            Examples: 
                | what           |
                | something      |
                | something else |
        """
    And all steps are bound and pass
    And row testing is disabled
    When I execute the tests
    Then the execution summary should contain
        | Succeeded | Ignored |
        | 0         | 2       |

Scenario: Should be able to ignore a scenario
    Given there is a SpecFlow project
    And there is a feature file in the project as
        """
            Feature: Simple Feature
            
            Scenario: First
                Given there is something
                Then something should happen

            @ignore
            Scenario: Second
                Given there is something
                Then something should happen
        """
    And all steps are bound and pass
    When I execute the tests
    Then the execution summary should contain
        | Succeeded | Ignored |
        | 1         | 1       |
