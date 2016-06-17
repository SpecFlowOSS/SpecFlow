Feature: AsyncSupport

Scenario: Should wait for aync step methods to complete before continuing to next step
	Given I am calling an async step method
	When I call the next async step method
	Then both step methods should have been called in order before I get to the last step method

Scenario: Step method should recieve exception from async method call
	Given I am testing a component with async methods
	When I call an async method that throws an exception
	Then the exception should be sent back to my step method