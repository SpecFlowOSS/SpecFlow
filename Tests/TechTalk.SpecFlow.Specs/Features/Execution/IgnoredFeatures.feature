Feature: Ignored Features

Scenario: Should be able to ignore all Scenarios in a feature
    Given there is a SpecFlow project
    And there is a feature file in the project as
        """
            @ignore
            Feature: Simple Feature
            
            Scenario: First
                Given there is something
                Then something should happen

            @ignore
            Scenario: Second
                Given there is something
                Then something should happen

            Scenario: Third
                Given there is something else
                Then something else should happen
        """
    And all steps are bound and pass
    And row testing is disabled
    When I execute the tests
    Then the execution summary should contain
        | Succeeded | Ignored |
        | 0         | 3       |

Scenario: Should be able to ignore all Scenarios and Scenario Outlines in a feature
    Given there is a SpecFlow project
    And there is a feature file in the project as
        """
            @ignore
            Feature: Simple Feature
            
            Scenario: First
                Given there is something
                Then something should happen

            Scenario Outline: First Outline
                Given there is something
                When I do <what>
                Then something should happen
            Examples: 
                | what           |
                | something      |
                | something else |

            @ignore
            Scenario Outline: Second Outline
                Given there is something
                When I do <what>
                Then something should happen
            Examples: 
                | what           |
                | something      |
                | something else |

            Scenario: Last
                Given there is something
                Then something should happen
        """
    And all steps are bound and pass
    And row testing is disabled
    When I execute the tests
    Then the execution summary should contain
        | Succeeded | Ignored |
        | 0         | 6       |