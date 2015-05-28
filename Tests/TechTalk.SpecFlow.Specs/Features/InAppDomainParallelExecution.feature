Feature: In-AppDomain Parallel Execution

Background: 
	Given there is a SpecFlow project
	And the project is configured to use the NUnit3 provider
    And the following binding class
        """
        [assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]
        """
	And the following step definition
         """
         public static int startIndex = 0;

         [When(@"I do something")]
		 public void WhenIDoSomething()
		 {
            var currentStartIndex = System.Threading.Interlocked.Increment(ref startIndex);
            Console.WriteLine("Start index: {0}", currentStartIndex);
            System.Threading.Thread.Sleep(200);
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

Scenario: Precondition: Tests run parallel with NUnit v3
    When I execute the tests with NUnit3
    Then the execution log should contain text 'Was parallel'

Scenario: Tests should be processed parallel without failure
    When I execute the tests with NUnit3
    Then the execution log should contain text 'Was parallel'
	And the execution summary should contain
		| Total | Succeeded |
		| 10    | 10        |

Scenario Outline: Before/After TestRun hook should only be executed once
    Given a hook 'HookFor<event>' for '<event>'
    When I execute the tests with NUnit3
    Then the execution log should contain text 'Was parallel'
    And the hook 'HookFor<event>' is executed once

Examples: 
	| event               |
	| BeforeTestRun       |
	| AfterTestRun        |


Scenario: TraceListener should be called synchronously
    Given the following binding class
        """
        public class NonThreadSafeTraceListener : TechTalk.SpecFlow.Tracing.ITraceListener
        {
            public int startIndex = 0;

            public void WriteTestOutput(string message)
            {
                var currentStartIndex = System.Threading.Interlocked.Increment(ref startIndex);
                Console.WriteLine("Listener Start index: {0}", currentStartIndex);
                System.Threading.Thread.Sleep(200);
                var afterStartIndex = startIndex;
                if (afterStartIndex != currentStartIndex)
                    throw new Exception("Listener was called in parallel");
            }

            public void WriteToolOutput(string message)
            {
                WriteTestOutput("-> " + message);
            }
        }
        """
    And the type 'SpecFlow.TestProject.NonThreadSafeTraceListener, SpecFlow.TestProject' is registered as 'TechTalk.SpecFlow.Tracing.ITraceListener' in SpecFlow runtime configuration
    When I execute the tests with NUnit3
    Then the execution log should contain text 'Was parallel'
	And the execution summary should contain
		| Total | Succeeded |
		| 10    | 10        |
