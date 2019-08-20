@MSTest
Feature: TestContext

Scenario: Should be able to access TestContext in Steps
    Given there is a SpecFlow project
    And a scenario 'Simple Scenario' as
        """
        When I do something
        """	
    And the following step definition
        """
        [When(@"I do something")]
        public void WhenIDoSomething()
        {
        System.Console.WriteLine(_scenarioContext.ScenarioContainer.Resolve<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>().TestName);
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |