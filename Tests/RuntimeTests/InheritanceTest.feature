Feature: Class inheritance
	In order to make step definition inheritance
	as a good software design developer
	I want to have derived class with some step definitions too


Scenario: Make instance just of derived, not base class
	Given I have created object of derived class
	And base constructor initializes class protected field
	Then derived class should have this field already initialized
	
Scenario Outline: Use same fields
	Given I have created object of derived class
	And base constructor initializes class protected field
	When I change value to <value> in derived class
	Then field in base class should also contain <value>
	
	Examples:
	| value |
	| 17    |
	| 135   |
