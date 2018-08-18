Feature: Goal: extend StepDefinitionReport
    find instances of step definition invocations when part of the definition text is a variable taken from a scenario outline example

Scenario: Report can handle scenario outlines

Given there is a SpecFlow project
And there is a feature file in the project as
        """
            Feature: Simple Feature
            Scenario Outline: Simple Scenario Outline
                Given there is <Col 1>

            Examples:
                | Col 1     |
                | Example 1 |
                | Example 2 |

        """
And the following step definitions
         """
            [Given("there is Example 1")]
            public void GivenThereIsExample1(string magic)
            {}

            [Given("there is Example 2")]
            public void GivenThereIsExample2()
            {}
         """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
    """
    Givens Step Definition Instances
    there is Example 1 [copy] 1 [show] Instances:
    there is Example 1 [copy] Simple Feature / Simple Scenario Outline (Scenario Outline Example)
    there is Example 2 [copy] 1 [show] Instances:
    there is Example 2 [copy] Simple Feature / Simple Scenario Outline (Scenario Outline Example)
    Whens Step Definition Instances
    Thens Step Definition Instances
    """

