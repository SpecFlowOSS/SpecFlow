Feature: Tagging Scenario Outline Examples
    As a developer
    I would like to be able to tag examples of the scenario outline
    So that I can write bindings that behave differently for the variants

Scenario: Examples can be tagged
    Given there is a feature file in the project as
        """
            Feature: Simple Feature
            Scenario Outline: Scenario Outline with Tagged Examples
                When I do <what>

            Examples: 
                | what           |
                | something      |
                | something else |

            @tag1
            Examples: 
                | what                |
                | something different |
        """
    And all steps are bound and pass
#this is a bug, the tagged examples can only be filtered with NUnit, if row testing is disabled
    And row testing is disabled
    When I execute the tests tagged with '@tag1'
    Then the execution summary should contain
        | Total |
        | 1     |

Scenario: Examples can be ignored
    Given there is a feature file in the project as
        """
            Feature: Simple Feature
            Scenario Outline: Scenario Outline with Tagged Examples
                When I do <what>

            Examples: 
                | what           |
                | something      |
                | something else |

            @ignore
            Examples: 
                | what                |
                | something different |
        """
    And all steps are bound and pass
#this is a bug, the tagged examples can only be filtered with NUnit, if row testing is disabled
    And row testing is disabled
    When I execute the tests
    Then the execution summary should contain
        | Total | Succeeded | Ignored |
        | 3     | 2         | 1       |

Scenario: Scenario Outline Examples can be tagged
    Given there is a feature file in the project as
        """
            Feature: Simple Feature
            Scenario Outline: Scenario Outline with Tagged Examples
                When I do <what>

            @en-gb
            Examples: 
                | what           |
                | something      |
                | something else |

            @fr-fr
            Examples: 
                | what           |
                | something      |
                | something else |
        """
    And the following step definition
         """
            [BeforeScenario]
            public void BeforeScenario()
            {
                var scenarioTags = _scenarioContext.ScenarioInfo.Tags;

                if (scenarioTags.Length == 0)
                {
                    throw new Exception("Scenario tags list should not be empty");
                }
            }
         """
    And all steps are bound and pass
    When I execute the tests
    Then the execution summary should contain
         | Succeeded |
         | 4         |