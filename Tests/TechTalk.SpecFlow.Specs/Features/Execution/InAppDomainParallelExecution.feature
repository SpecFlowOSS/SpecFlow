@XUnit @NUnit
#parallel execution doesn't work currently with MsTest v2

Feature: In-AppDomain Parallel Execution

Background:
    Given there is a SpecFlow project
    And parallel execution is enabled

    And the following step definition
         """
         public static int startIndex = 0;

         [When(@"I do something")]
         public void WhenIDoSomething()
         {
            var currentStartIndex = System.Threading.Interlocked.Increment(ref startIndex);
            Console.WriteLine("Start index: {0}", currentStartIndex);
            System.Threading.Thread.Sleep(500);
            var afterStartIndex = startIndex;
            if (afterStartIndex == currentStartIndex)
                Console.WriteLine("Was not parallel");
            else
                Console.WriteLine("Was parallel");
         }
         """
    Given there is a feature file in the project as
        """
        Feature: Feature 1
        Scenario Outline: Simple Scenario Outline
          When I do something

      Examples:
        | Count |
        | 1     |
        | 2     |
        | 3     |
        | 4     |
        | 5     |
        """
    Given there is a feature file in the project as
        """
        Feature: Feature 2
        Scenario Outline: Simple Scenario Outline
          When I do something

      Examples:
        | Count |
        | 1     |
        | 2     |
        | 3     |
        | 4     |
        | 5     |
        """

Scenario: Precondition: Tests run parallel 
    When I execute the tests
    Then the execution log should contain text 'Was parallel'

Scenario: Tests should be processed parallel without failure
    When I execute the tests
    Then the execution log should contain text 'Was parallel'
    And the execution summary should contain
        | Total | Succeeded |
        | 10    | 10        |

Scenario Outline: Before/After TestRun hook should only be executed once
    Given a hook 'HookFor<event>' for '<event>'
    When I execute the tests
    Then the execution log should contain text 'Was parallel'
    And the hook 'HookFor<event>' is executed once

Examples:
    | event               |
    | BeforeTestRun       |
    | AfterTestRun        |


Scenario: TraceListener should be called synchronously
    Given the following binding class
        """
        using System;
        using System.IO;
        using System.Threading;

        public class NonThreadSafeTraceListener : TechTalk.SpecFlow.Tracing.ITraceListener
        {
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

            public void WriteTestOutput(string message)
            {
                // Attempts to enter the semaphore immediately
                // If not successfully entered, the exception is thrown
                if (!_semaphore.Wait(0))
                {
                    throw new Exception("Listener was called in parallel");
                }

                System.Diagnostics.Debug.WriteLine("NonThreadSafeTraceListener: {0}", message);
                string filePath = Path.Combine(@"$ProjectDir$", "NonThreadSafeTraceListener.log");
                File.AppendAllText(filePath, "NonThreadSafeTraceListener: " + message + Environment.NewLine);
                Thread.Sleep(100);

                _semaphore.Release();
            }

            public void WriteToolOutput(string message)
            {
                WriteTestOutput("-> " + message);
            }
        }
        """
    And there is a feature file in the project as
        """
        Feature: Feature 5
        Scenario Outline: Simple Scenario Outline
          When I do something

      Examples:
        | Count |
        | 1     |
        | 2     |
        | 3     |
        | 4     |
        | 5     |
        """
    And there is a feature file in the project as
        """
        Feature: Feature 3
        Scenario Outline: Simple Scenario Outline
          When I do something

      Examples:
        | Count |
        | 1     |
        | 2     |
        | 3     |
        | 4     |
        | 5     |
        """
    And there is a feature file in the project as
        """
        Feature: Feature 4
        Scenario Outline: Simple Scenario Outline
          When I do something

      Examples:
        | Count |
        | 1     |
        | 2     |
        | 3     |
        | 4     |
        | 5     |
        """
    And the type 'NonThreadSafeTraceListener' is registered as 'TechTalk.SpecFlow.Tracing.ITraceListener' in SpecFlow runtime configuration    
    When I execute the tests
    Then the execution log should contain text 'Was parallel'
    Then the log file 'NonThreadSafeTraceListener.log' should contain text 'NonThreadSafeTraceListener:'
    Then the log file 'NonThreadSafeTraceListener.log' should contain the text 'NonThreadSafeTraceListener:' 51 times
    #25* start call
    #25* done call
    #1 which configuration is used
    And the execution summary should contain
        | Total | Succeeded |
        | 25    | 25        |


Scenario Outline: Current context cannot be used in multi-threaded execution
    Given there is a feature file in the project as
        """
        Feature: Feature with <context>.Current
        Scenario: Simple Scenario
          When I use <context>.Current
        """
    And the following step definition
         """
         [When(@"I use <context>.Current")]
         public void WhenIUseContextCurrent()
         {
            System.Threading.Thread.Sleep(200);
            Console.WriteLine(<context>.Current);
         }
         """
    When I execute the tests
    Then the execution log should contain text 'Was parallel'
    And the execution summary should contain
        | Failed |
        | 1      |

Examples:
    | context             |
    | ScenarioContext     |
    | FeatureContext      |
    | ScenarioStepContext |