@xUnit @NUnit3 @MSTest
Feature: In-AppDomain Parallel Execution

Background:
    Given there is a SpecFlow project
    And parallel execution is enabled
    And the following binding class 
        """
        using TechTalk.SpecFlow;
        using TechTalk.SpecFlow.Tracing;

        [Binding]
        public class TraceSteps
        {
            private readonly ITraceListener _traceListener;
            private readonly ITestRunner _testRunner;

            public TraceSteps(ITraceListener traceListener, ITestRunner testRunner)
            {
                _traceListener = traceListener;
                _testRunner = testRunner;
            }

            public static int startIndex = 0;

            [When(@"I do something")]
            void WhenIDoSomething()
            {
                var currentStartIndex = System.Threading.Interlocked.Increment(ref startIndex);
                _traceListener.WriteTestOutput($"Start index: {currentStartIndex}, Worker: {_testRunner.TestWorkerId}");
                System.Threading.Tasks.Task.Delay(500).Wait();
                var afterStartIndex = startIndex;
                if (afterStartIndex == currentStartIndex)
                {
                    _traceListener.WriteTestOutput("Was not parallel");
                }
                else
                {
                    _traceListener.WriteTestOutput("Was parallel");
                }
            }
        }
        """

    And there is a feature file in the project as
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
    And there is a feature file in the project as
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