Feature: Async Exception

Scenario: Some basic scenario with synchronous steps
	Given some basic setup
	When some basic synchronous action
	Then some basic result

@SomeTag
Scenario: Some basic scenario with an async step
	Given some basic setup
	When some basic async action
	Then some basic result
