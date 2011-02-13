Feature: Step Argument Transformations
	In order to reduce the amount of code and repetitive tasks in my steps
	As a programmer
	I want to define reusable transformations for my step arguments
	
Scenario: Steps with non-string arguments 
	Given Dan has been registered at date 2003/03/13 
	And Aslak has been registered at terminal 2 
	And Bob has booked a flight
	|Origin	|Destination		|Departure Date		|
	|Hull	|Anywhere else		|2011/01/01			|

Scenario: Multi-line text arguments can be converted to non-string step parame
	Given I have a step binding with an XmlDocument parameter
	And a step argument converter from string to XmlDocument
	When a step is executed that macthes to the binding with a long XML argument
		"""
			<Root>
				<Child attr="value" />
			</Root>
		"""
	Then the parsed XML is passed to the step binding
