namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;

    using TechTalk.SpecFlow.Infrastructure;
    using TechTalk.SpecFlow.Specs.Drivers;

    [Binding]
    public class AsyncSteps
    {
        private readonly TestExecutionResult testExecutionResult;
        private readonly ScenarioContext scenarioContext;

        public AsyncSteps(TestExecutionResult testExecutionResult, ScenarioContext scenarioContext)
        {
            
            this.testExecutionResult = testExecutionResult;
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
            this.scenarioContext.ContainsKey("GivenStepCompleted").Should().BeTrue();
            this.scenarioContext.ContainsKey("WhenStepCompleted").Should().BeTrue();
            ((long)this.scenarioContext["WhenStepCompleted"]).Should().BeGreaterThan((long)this.scenarioContext["GivenStepCompleted"]);
        }

        [Then(@"the exception should be sent back to my step method")]
        public void ThenTheExceptionShouldBeSentBackToMyStepMethod()
        {
            this.scenarioContext.ContainsKey("Exception").Should().BeTrue();
            ((Exception)this.scenarioContext["Exception"]).Message.Should().Be("This exception should bubble up.");
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
}