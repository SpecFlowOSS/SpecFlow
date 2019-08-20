Feature: Calling Steps from StepDefinitions
	In order to create steps of a higher abstraction
	As a developer
	I want reuse other steps in my step definitions

Scenario Outline: Other step definition can be called when derive from Steps base class
	Given the following binding class
         """
         using TechTalk.SpecFlow;

	     [Binding]
	     public class CallingStepsFromStepDefinitionSteps : Steps
         {
			[<step type>(@"I have entered (\d+) into the calculator")]
		    public void GivenIHaveEntered(int number)
			{
				//...
			}

			[<step type>(@"I have entered (\d+) and (\d+) into the calculator")]
		    public async Task GivenIHaveEntered(int n1, int n2)
			{
				<step type>Async(string.Format("I have entered {0} into the calculator", n1));
				<step type>Async(string.Format("I have entered {0} into the calculator", n2));
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

Scenario: When I call other steps of a different type it shouldn't change the type of the next and step in my feature
	Given the following binding class
         """        
         using TechTalk.SpecFlow;

	     [Binding]
	     public class CallingStepsFromStepDefinitionSteps : Steps
         {
			[Given(@"I have a given step")]
		    public void GivenIHaveEntered()
			{
				//...
			}

			[When(@"I call a step of a different type")]
		    public void WhenICallAStepOfADifferentType()
			{
				//...
			}

			[Given(@"I want to call another given step")]
		    public void GivenIWantToCallAnotherGivenStep()
			{
				//...
			}

			[Given(@"I called some steps of different types")]
		    public async Task GivenIHaveCalledStepsOfDifferentTypes()
			{
				await GivenAsync("I have a given step");
				await WhenAsync("I call a step of a different type");
			}
		 }	
         """
	And a scenario 'Simple Scenario' as
         """
			Given I called some steps of different types
			And I want to call another given step
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |


Scenario: When I call other steps of a different types in a hierarchy it shouldn't change the type of the next and step in my feature
	Given the following binding class
         """            
         using TechTalk.SpecFlow;

	     [Binding]
	     public class CallingStepsFromStepDefinitionSteps : Steps
         {
			[Given(@"I have a given step")]
		    public void GivenIHaveEntered()
			{
				//...
			}

			[Given(@"I called a starting step")]
		    public void GenericStepName7()
			{
				//...
			}

			[When(@"I call a step of a different type")]
		    public void WhenICallAStepOfADifferentType()
			{
				//...
			}

			[Given(@"I want to call another given step")]
		    public void GivenIWantToCallAnotherGivenStep()
			{
				//...
			}

			[Given(@"I called some steps of different types")]
		    public async Task GenericStepName6()
			{
				await GivenAsync("I have a given step");
				await WhenAsync("I call a step of a different type");
			}

			[When(@"I have a when step")]
		    public void GenericStepName8()
			{
				//...
			}

			[When(@"this step is called")]
		    public async Task GenericStepName2()
			{
				await ThenAsync("this calls anotherStep");
			}

			[Then(@"this calls anotherStep")]
		    public void GenericStepName1()
			{
				//..
			}

			[Given(@"then this step is called")]
		    public void GenericStepName3()
			{
				//..
			}

			[When(@"it calls a step which calls other steps")]
		    public async Task GenericStepName4()
			{
				await WhenAsync("this step is called");
				await GivenAsync("then this step is called");
			}

			[Then(@"it should still find this step")]
		    public void GenericStepName5()
			{
				//..
			}
		 }	
         """
	And a scenario 'Simple Scenario' as
         """
			Given I called a starting step
			And I called some steps of different types
			And I want to call another given step
			When I have a when step
			And it calls a step which calls other steps
			Then it should still find this step
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |