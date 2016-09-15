Feature: LanguageSupport
	

Scenario Outline: single Scenario
    Given I have a '<Language>' test project
        And there is a feature file in the project as
            """
            Feature: Simple Feature
            Scenario: Simple Scenario
                When I do something
            """
        And all steps are bound and pass

    When I execute the tests

    Then the execution summary should contain
        | Total | Succeeded |
        | 1     | 1         |

Examples: 
    | Language |
    | C#       |
    | VB.Net   |