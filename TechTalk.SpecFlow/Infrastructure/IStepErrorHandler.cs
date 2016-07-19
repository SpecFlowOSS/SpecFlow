using System;
using System.Linq;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class StepFailureEventArgs 
    {
        public IStepDefinitionBinding StepDefiniton { get; set; }
        public StepContext StepContext { get; set; }
        public Exception Exception { get; set; }

        public bool IsHandled { get; set; }

        public StepFailureEventArgs(IStepDefinitionBinding stepDefiniton, StepContext stepContext, Exception exception)
        {
            IsHandled = false;

            StepDefiniton = stepDefiniton;
            StepContext = stepContext;
            Exception = exception;
        }
    }

    public interface IStepErrorHandler
    {
        void OnStepFailure(TestExecutionEngine sender, StepFailureEventArgs args);
    }
}
