@xUnit
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
        ScenarioContext.Current.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>().WriteLine("something");
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    And the execution log should contain text 'something'


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
    And the execution log should contain text 'something'

Scenario: Usage of collection attribute adds category
    Given there is a SpecFlow project
    And a scenario named 'Simple Scenario' with collection tag 'SampleCollection' as
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
    When I execute the tests tagged with '@xunit:collection\(SampleCollection\)'
    Then the execution summary should contain
        | Succeeded |
        | 1         |

Scenario: Usage of collection attribute injects collection's TFixture
    Given there is a SpecFlow project
    And a scenario named 'Simple Scenario' with collection tag 'SampleCollection' as
        """
        Then collection TFixture is initialized
        """	
    And the following binding class
        """
        using System;
        using TechTalk.SpecFlow;
        using Xunit;
        
        [CollectionDefinition(nameof(SampleCollection))]
        public class SampleCollection : ICollectionFixture<SampleFixture>
        {
            // This class has no code, and is never created. Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
        }
        public class SampleFixture
        {
            // Will be initialized and added to DI container while collection initialization
        }

        [Binding]
        public class StepsWithScenarioContext
        {
            private SampleFixture _sampleFixture;
            private ScenarioContext _scenarioContext;
            private Xunit.Abstractions.ITestOutputHelper _output;
            public StepsWithScenarioContext(ScenarioContext scenarioContext, SampleFixture sampleFixture)
            {
                _scenarioContext = scenarioContext;
                _output = _scenarioContext.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>();
                _sampleFixture = sampleFixture;
            }

            [Then(@"collection TFixture is initialized")]
            public void ThenCollectionTFixtureInitialized()
            {
                if (_sampleFixture != null) 
                {
                    _output.WriteLine("TFixture initialization works");
                }
            }
        }
        """	
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    And the execution log should contain text 'TFixture initialization works'
