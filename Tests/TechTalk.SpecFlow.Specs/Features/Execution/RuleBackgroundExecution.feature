@gherkin6
Feature: Rule Background Steps Execution

    Scenario: Should be able to execute scenarios in Rules that have backgrounds
  	    Given the following binding class
		     """
             using TechTalk.SpecFlow;

		     [Binding]
		     public class RuleSteps1
		     {

			    [Given("something first as background")]
			    public void GivenSomethingFirst() {
                    global::Log.LogStep();
                }

                [When("I do something")]
                public void WhenSomethingDone() {
                    global::Log.LogStep();
                }
		     }
		     """
        Given there is a feature file in the project as
            """
                Feature: Simple Feature
                Rule: first rule
                Background: first rule background
                    Given something first as background
            
                Scenario: Scenario for the first rule
                    When I do something
            """


        When I execute the tests
        Then the binding method 'GivenSomethingFirst' is executed


    Scenario: Should be able to execute backgrounds from multiple Rules
  	    Given the following binding class
		     """
             using TechTalk.SpecFlow;

		     [Binding]
		     public class RuleSteps2
		     {
                private bool first_background_executed = false;
                private bool second_background_executed = false;

			    [Given("a first thing as background")]
			    public void GivenaFirst() {
                    global::Log.LogStep();
                    first_background_executed = true;
                }

                [Then("the first of two background item was executed")]
                public void ThenFirstOfTwoWasExecuted() {
                    global::Log.LogStep();
                    if (!first_background_executed) {
                        throw new ApplicationException("First Background Step should have been executed");
                    }
                }

                [Then("the first background item was not executed")]
                public void ThenFirstWasNotExecuted() {
                    global::Log.LogStep();
                    if (first_background_executed) { 
                        throw new ApplicationException("First Background Step should not have been executed");
                    }
                }

			    [Given("something second as background")]
			    public void GivenSomethingSecond() {
                    global::Log.LogStep();
                    second_background_executed = true;
                }

                [Then("the second background item was executed")]
                public void ThenSecondWasExecuted() {
                    global::Log.LogStep();
                    if (!second_background_executed) { 
                        throw new ApplicationException("Second Background Step should have been executed");
                    }
                }

                [Then("the second background item was not executed")]
                public void ThenSecondWasNotExecuted() {
                    global::Log.LogStep();
                    if (second_background_executed) { 
                        throw new ApplicationException("Second Background Step should not have been executed");
                    }
                }

                [When("I do something")]
                public void WhenSomethingDone() {
                    global::Log.LogStep();
                }

		     }
		     """
        Given there is a feature file in the project as
            """
                Feature: Simple Feature
                Rule: first rule
                    Background: first rule background
                        Given a first thing as background
            
                    Scenario: Scenario for the first rule
                        When I do something
                        Then the first of two background item was executed
                        And the second background item was not executed

                Rule: second rule
                    Background: bg for second rule
                        Given something second as background

                    Scenario:Scenario for the second rule
                        When I do something
                        Then the second background item was executed
                        And the first background item was not executed

            """

        When I execute the tests
        Then all tests should pass
