Feature: In-AppDomain Parallel Execution

Scenario: Precondition: Tests run parallel with NUnit v3
	Given there is a SpecFlow project
	And the project is configured to use the NUnit3 provider
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
            NUnit.Framework.Assert.AreEqual(currentStartIndex, afterStartIndex);
		 }
         """
    Given the following binding class
        """
        [assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]
        """
    When I execute the tests with NUnit3
    Then the execution log should contain text 'Was parallel'