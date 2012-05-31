Feature: Different step arguments

Scenario: Steps can be defined without argument
	Given the following step definition
        """
		[When(@"I do something")]
		public void WhenIDoSomething()
		{
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
         """
	When I execute the tests
	Then all tests should pass

Scenario: Steps can be defined with table argument
	Given the following step definition
        """
		[When(@"I do something")]
		public void WhenIDoSomething(Table table)
		{
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
			| foo |
			| bar |
         """
	When I execute the tests
	Then all tests should pass


Scenario: Steps can be defined with multiline text argument
	Given the following step definition
        """
		[When(@"I do something")]
		public void WhenIDoSomething(string text)
		{
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
			'''
				<Root>
					<Child attr="value" />
				</Root>
			'''
         """
	When I execute the tests
	Then all tests should pass


Scenario: Steps can be defined with both table and multiline text argument
	Given the following step definition
        """
		[When(@"I do something")]
		public void WhenIDoSomething(string text, Table table)
		{
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
			'''
				<Root>
					<Child attr="value" />
				</Root>
			'''
			| foo |
			| bar |
         """
	When I execute the tests
	Then all tests should pass

Scenario: Step parameters in the step definitions have to be declared before the multiline text and table parameters
	Given the following step definition
        """
		[When(@"I (.*) something")]
		public void WhenIDoSomething(string what, Table table)
		{
		}
        """
	And a scenario 'Simple Scenario' as
         """
         When I do something
			| foo |
			| bar |
         """
	When I execute the tests
	Then all tests should pass
