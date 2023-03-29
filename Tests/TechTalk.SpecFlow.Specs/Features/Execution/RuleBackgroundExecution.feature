@gherkin6
Feature: Rule Background Steps Are Added to Scenarios Belonging to Rules

    Scenario: Should be able to execute scenarios in Rules that have backgrounds
  	    Given the following binding class
		    """
                using TechTalk.SpecFlow;

                [Binding]
                public class RuleStepsForFirstScenario
                {

                    [Given("a background step")]
                    public void TheBackgroundStep()
                    {
                        global::Log.LogStep();
                    }

                    [When("I do something")]
                    public void WhenSomethingDone()
                    {
                        global::Log.LogStep();
                    }
                }
            """

        Given there is a feature file in the project as
            """
                Feature: Simple Feature
                    Rule: A Rule
                        Background: rule background
                            Given a background step
            
                        Scenario: Scenario for the first rule
                            When I do something
            """

        When I execute the tests
        Then the binding method 'TheBackgroundStep' is executed


    Scenario: Should be able to execute backgrounds from multiple Rules
  	    Given the following binding class
		     """
                using System;
                using TechTalk.SpecFlow;

                internal class StepInvocationTracker
                {
                    private bool first_background_step_executed = false;
                    private bool second_backgroun_step_executed = false;

                    public void MarkFirstStepAsExecuted() => first_background_step_executed = true;

                    public void MarkSecondStepAsExecuted() => second_backgroun_step_executed = true;

                    public bool WasFirstStepInvoked => first_background_step_executed;
                    public bool WasSecondStepInvoked => second_backgroun_step_executed;

                }

                [Binding]
                public class RuleStepsForFeatureContainingMultipleRules
                {

                    private StepInvocationTracker invocationTracker = new StepInvocationTracker();

                    [Given("a background step for the first rule")]
                    public void GivenaFirst()
                    {
                        global::Log.LogStep();
                        invocationTracker.MarkFirstStepAsExecuted();
                    }

                    [Then("the first background step was executed")]
                    public void ThenFirstOfTwoWasExecuted()
                    {
                        global::Log.LogStep();
                        if (!invocationTracker.WasFirstStepInvoked)
                        {
                            throw new ApplicationException("First Background Step should have been executed");
                        }
                    }

                    [Then("the step from the first background was not executed")]
                    public void ThenFirstWasNotExecuted()
                    {
                        global::Log.LogStep();
                        if (invocationTracker.WasFirstStepInvoked)
                        {
                            throw new ApplicationException("First Background Step should not have been executed");
                        }
                    }

                    [Given("a background step for the second rule")]
                    public void GivenSecondBackgroundStepExecuted()
                    {
                        global::Log.LogStep();
                        invocationTracker.MarkSecondStepAsExecuted();
                    }

                    [Then("the second background step was executed")]
                    public void ThenSecondWasExecuted()
                    {
                        global::Log.LogStep();
                        if (!invocationTracker.WasSecondStepInvoked)
                        {
                            throw new ApplicationException("Second Background Step should have been executed");
                        }
                    }

                    [Then("the step from the second background was not executed")]
                    public void ThenSecondWasNotExecuted()
                    {
                        global::Log.LogStep();
                        if (invocationTracker.WasSecondStepInvoked)
                        {
                            throw new ApplicationException("Second Background Step should not have been executed");
                        }
                    }
                }
		     """
        Given there is a feature file in the project as
            """
                Feature: A Feature with multiple Rules, each with their own Backgrounds

                    Rule: first Rule
                        Background: first Rule's background
                            Given a background step for the first rule
            
                        Scenario: Scenario for the first rule
                            Then the first background step was executed
                            And the step from the second background was not executed

                    Rule: second Rule
                        Background: second Rule's background
                            Given a background step for the second rule

                        Scenario:Scenario for the second rule
                            Then the second background step was executed
                            And the step from the first background was not executed

            """

        When I execute the tests
        Then all tests should pass
