@XUnit
Feature: XUnit v2 unit test provider
    
Scenario: Should be able to log custom messages
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
        ScenarioContext.Current.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>().WriteLine("hello");
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |


Scenario: Should be able to log custom messages using context injection
    Given there is a SpecFlow project
    And the following binding class
        """
        using System;
        using TechTalk.SpecFlow;

        [Binding]
        public class StepsWithScenarioContext
        {
            private ScenarioContext _scenarioContext;
            private Xunit.Abstractions.ITestOutputHelper _output;
            public StepsWithScenarioContext(ScenarioContext scenarioContext)
            {
                _scenarioContext = scenarioContext;
                _output = _scenarioContext.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>();
            }

            [When(@"I do something")]
            public void WhenIDoSomething()
            {
                _output.WriteLine("something");
            }
        }
        """	
    And a scenario 'Simple Scenario' as
        """
        When I do something         
        """
    When I execute the tests
    Then the execution summary should contain
         | Succeeded |
         | 1         |