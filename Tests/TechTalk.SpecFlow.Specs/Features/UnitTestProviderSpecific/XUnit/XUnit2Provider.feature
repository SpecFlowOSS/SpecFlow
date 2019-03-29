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
        ScenarioContext.Current.ScenarioContainer.Resolve<Xunit.Abstractions.ITestOutputHelper>().WriteLine("F89FFFA1-88CD-40C8-B0E2-2B167E1F81A2");
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    And the execution log should contain text 'F89FFFA1-88CD-40C8-B0E2-2B167E1F81A2'


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
                _output.WriteLine("EB7C1291-2C44-417F-ABB7-A5154843BC7B");
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
    And the execution log should contain text 'EB7C1291-2C44-417F-ABB7-A5154843BC7B'

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
                    _output.WriteLine("TFixture initialization works 5C3548BE-F19C-485C-8A9F-E2BF1C5BF1B3");
                }
            }
        }
        """	
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    And the execution log should contain text 'TFixture initialization works 5C3548BE-F19C-485C-8A9F-E2BF1C5BF1B3'
