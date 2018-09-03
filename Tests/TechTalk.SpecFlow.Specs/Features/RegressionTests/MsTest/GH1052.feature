@MSTest
Feature: GH1052

Wrong scenario context injected when running tests in parallel using NUnit (also specrun) - https://github.com/techtalk/SpecFlow/issues/1052


Scenario: GH1052

    Given the following binding class
        """
        using Microsoft.VisualStudio.TestTools.UnitTesting;
        [assembly: Parallelize(Workers = 2, Scope = ExecutionScope.ClassLevel)]

        """
    And there is a feature file in the project as
         """
         Feature: Parallel Test
             In order to know my illustrate a bug
             As a user of specflow
             I need to execute these scenarios

         Scenario Outline: The first scenario example
             Given I'm illustrating the issue

         Examples:
            | Title | 
            | A     | 
            | B     | 
            | C     | 
            | D     | 
            | E     | 
            | F     | 
            | G     | 
            | H     | 
            | I     | 
            | J     | 
            | K     | 
            | L     | 
            | M     | 
            | N     | 
            | O     | 
            | P     | 
            | Q     | 
            | R     | 
            | S     | 


         Scenario Outline: The second scenario example
             Given I'm illustrating the issue

         Examples:
            | Title | 
            | A     | 
            | B     | 
            | C     | 
            | D     | 
            | E     | 
            | F     | 
            | G     | 
            | H     | 
            | I     | 
            | J     | 
            | K     | 
            | L     | 
            | M     | 
            | N     | 
            | O     | 
            | P     | 
            | Q     | 
            | R     | 
            | S     | 
         
         
         """

    And the following binding class
        """
        using System;
        using System.Threading;
        using TechTalk.SpecFlow;
        using TechTalk.SpecFlow.Infrastructure;

        [Binding]
        public class LosingTheWillToLiveSteps
        {
            private readonly ScenarioContext _scenarioContext;
            private readonly ISpecFlowOutputHelper _specFlowOutputHelper;

            public LosingTheWillToLiveSteps(ScenarioContext scenarioContext, ISpecFlowOutputHelper specFlowOutputHelper)
            {
                _scenarioContext = scenarioContext;
                _specFlowOutputHelper = specFlowOutputHelper;
            }

            [BeforeScenario]
            public void BeforeScenario()
            {
                Console.Out.WriteLine("BeforeScenario");
                try
                {
                    var id = Guid.NewGuid().ToString();
                    _scenarioContext.Add("ID", id);
                    WriteId();
                }
                catch (Exception e)
                {
                    _specFlowOutputHelper.WriteLine($"Error adding id {e.Message}");
                }
            }

            [AfterScenario]
            public void AfterScenario()
            {
                try
                {
                    WriteId();
                }
                catch (Exception e)
                {
                    _specFlowOutputHelper.WriteLine($"Error clearing up the Scenario {e.Message}");
                }
            }

            [Given(@"I'm illustrating the issue")]
            public void GivenImIllustratingTheIssue()
            {
                WriteId();
            }

            private void WriteId()
            {
                try
                {
                    _specFlowOutputHelper.WriteLine($"Context ID {_scenarioContext["ID"].ToString()}");
                    _specFlowOutputHelper.WriteLine($"ManagedThreadId {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    _specFlowOutputHelper.WriteLine($"Error- {e.Message}");
                }
            }

        }
         
        """
    
    When I execute the tests
    Then every scenario has it's individual context id
    And the execution summary should contain
         | Total | Succeeded | Failed |
         | 38    | 38        | 0      |
