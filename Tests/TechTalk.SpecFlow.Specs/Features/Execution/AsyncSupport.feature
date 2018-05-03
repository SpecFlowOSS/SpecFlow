Feature: AsyncSupport

Background:
	Given the following binding class
		"""
		using System;
        using System.Threading.Tasks;
        using System.Xml.Linq;
        using FluentAssertions;
        using TechTalk.SpecFlow;

        [Binding]
        public class AsyncSteps
        {
	        private readonly ScenarioContext scenarioContext;

	        public AsyncSteps(ScenarioContext scenarioContext)
	        {
        
		        this.scenarioContext = scenarioContext;
	        }

	        [Given(@"I am calling an async step method")]
	        public async Task GivenIAmCallingAnAsyncStep()
	        {
		        await Task.Delay(250);

		        this.scenarioContext["GivenStepCompleted"] = DateTime.Now.Ticks;
	        }

	        [Given(@"I am testing a component with async methods")]
	        public void GivenIAmTestingAComponentWithAsyncMethods()
	        {
		        this.scenarioContext["Component"] = new AsyncComponent();
	        }

	        [When(@"I call the next async step method")]
	        public async Task WhenICallTheNextAsyncStepMethod()
	        {
		        await Task.Delay(100);

		        this.scenarioContext["WhenStepCompleted"] = DateTime.Now.Ticks;
	        }

	        [When(@"I call an async method that throws an exception")]
	        public async Task WhenICallAnAsyncMethodThatThrowsAnException()
	        {
		        try
		        {
			        await ((AsyncComponent)this.scenarioContext["Component"]).AsyncMethod();
		        }
		        catch (Exception ex)
		        {
			        this.scenarioContext["Exception"] = ex;
		        }
	        }

	        [Then(@"both step methods should have been called in order before I get to the last step method")]
	        public void ThenBothStepMethodsShouldHaveBeenCalledInOrderBeforeIGetToTheLastStepMethod()
	        {
		        this.scenarioContext.Should().ContainKey("GivenStepCompleted");
		        this.scenarioContext.Should().ContainKey("WhenStepCompleted");
		        var givenStepCompleted = (long)this.scenarioContext["GivenStepCompleted"];
		        var whenStepCompleted = (long)this.scenarioContext["WhenStepCompleted"];
		        whenStepCompleted.Should().BeGreaterThan(givenStepCompleted);
	        }

	        [Then(@"the exception should be sent back to my step method")]
	        public void ThenTheExceptionShouldBeSentBackToMyStepMethod()
	        {
		        this.scenarioContext.Should().ContainKey("Exception");
		        var exception = (Exception)this.scenarioContext["Exception"];
		        exception.Message.Should().Be("This exception should bubble up.");
	        }

	        private class AsyncComponent
	        {
		        public async Task AsyncMethod()
		        {
			        await Task.Delay(100);

			        throw new Exception("This exception should bubble up.");
		        }
	        }
        }
		"""

Scenario: Should wait for aync step methods to complete before continuing to next step
	Given a scenario 'Wait for async step methods to complete before continuing to next step' as
		"""
		Given I am calling an async step method
		When I call the next async step method
		Then both step methods should have been called in order before I get to the last step method

		"""
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Step method should recieve exception from async method call
	Given a scenario 'Step method should recieve exception from async method call' as
		"""
		Given I am testing a component with async methods
		When I call an async method that throws an exception
		Then the exception should be sent back to my step method

		"""
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
