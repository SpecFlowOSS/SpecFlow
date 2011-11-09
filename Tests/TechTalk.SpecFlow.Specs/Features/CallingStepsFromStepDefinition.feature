Feature: Calling Steps from StepDefinitions
	In order to create steps of a higher abstraction
	As a developer
	I want reuse other steps in my step definitions

Scenario Outline: Other step definition can be called when derive from Steps base class
	Given the following binding class
         """
	     [Binding]
	     public class CallingStepsFromStepDefinitionSteps : Steps
         {
			[<step type>(@"I have entered (\d+) into the calculator")]
		    public void GivenIHaveEntered(int number)
			{
				//...
			}

			[<step type>(@"I have entered (\d+) and (\d+) into the calculator")]
		    public void GivenIHaveEntered(int n1, int n2)
			{
				<step type>(string.Format("I have entered {0} into the calculator", n1));
				<step type>(string.Format("I have entered {0} into the calculator", n2));
			}
		 }	
         """
	And a scenario 'Simple Scenario' as
         """
			<step type> I have entered 2 and 3 into the calculator
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Examples: 
	| step type |
	| Given     |
	| When      |
	| Then      |