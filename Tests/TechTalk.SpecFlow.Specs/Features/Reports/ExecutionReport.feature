Feature: Generating HTML report from NUnit/MsTest execution result

Background: 
	Given there is a feature 'Feature with successful scenarios' with 2 passing 0 failing 0 pending and 0 ignored scenarios
	And there is a feature 'Feature with failing scenarios' with 0 passing 1 failing 1 pending and 1 ignored scenarios


Scenario: Summary is included in the HTML result (NUnit)
	Given there are NUnit test execution results for the project
	When I generate SpecFlow NUnit execution report
	Then the generated report contains
	"""
		Summary 
		Features	Success rate	Scenarios	Success		Failed	Pending		Ignored 
		2 features	40%				5			2			1		1			1 
	"""

@mstest
Scenario: Summary is included in the HTML result (MsTest)
	Given there are MsTest test execution results for the project
	When I generate SpecFlow MsTest execution report
	Then the generated report contains
	"""
		Summary 
		Features	Success rate	Scenarios	Success		Failed	Pending		Ignored 
		2 features	50%				4			2			1		1			0 
	"""

Scenario: Feature summary is included in the HTML result (NUnit)
	Given there are NUnit test execution results for the project
	When I generate SpecFlow NUnit execution report
	Then the generated report contains
	"""
		Feature Summary
		Feature								Success rate	Scenarios	Success		Failed	Pending		Ignored		
		Feature with failing scenarios		0%				3			0			1		1			1
		Feature with successful scenarios	100%			2			2			0		0			0
	"""

@mstest
Scenario: Feature summary is included in the HTML result (MsTest)
	Given there are MsTest test execution results for the project
	When I generate SpecFlow MsTest execution report
	Then the generated report contains
	"""
		Feature Summary
		Feature								Success rate	Scenarios	Success		Failed	Pending		Ignored		
		Feature with failing scenarios		0%				2			0			1		1			0
		Feature with successful scenarios	100%			2			2			0		0			0
	"""

Scenario Outline: Successful test output is included in the HTML result
	Given there are <unittest> test execution results for the project
	When I generate SpecFlow <unittest> execution report
	Then the generated report contains
	"""
		When the step pass in Feature with successful scenarios
		-> done: 
	"""
Examples:
	| unittest	|
	| NUnit		|
@mstest
Examples: MsTest
	| unittest	|
	| MsTest	|

Scenario Outline: Pending test output is included in the HTML result
	Given there are <unittest> test execution results for the project
	When I generate SpecFlow <unittest> execution report
	Then the generated report contains
	"""
		When the step is pending
		-> No matching step definition found for the step. Use the following code to create one:
	"""
	And the generated report contains
	"""
		 [When(@"the step is pending")]
         public void WhenTheStepIsPending()
         {
             ScenarioContext.Current.Pending();
         }
    """
Examples:
	| unittest	|
	| NUnit		|
@mstest
Examples: MsTest
	| unittest	|
	| MsTest	|

Scenario Outline: Failing test output is included in the HTML result
	Given there are <unittest> test execution results for the project
	When I generate SpecFlow <unittest> execution report
	Then the generated report contains
	"""
		When the step fail in Feature with failing scenarios
		-> error:
	"""
	And the generated report contains
	"""
		simulated failure
	"""
Examples:
	| unittest	|
	| NUnit		|
@mstest
Examples: MsTest
	| unittest	|
	| MsTest	|

Scenario Outline: Failing test exception is included in the HTML result
	Given there are <unittest> test execution results for the project
	When I generate SpecFlow <unittest> execution report
	Then the generated report contains
	"""
		system.exception
	"""
	And the generated report contains
	"""
		simulated failure
	"""
Examples:
	| unittest	|
	| NUnit		|
@mstest
Examples: MsTest
	| unittest	|
	| MsTest	|

