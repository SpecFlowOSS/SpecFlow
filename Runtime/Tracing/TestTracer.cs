using System;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    public interface ITestTracer
    {
        void TraceStep(StepArgs stepArgs, bool showAdditionalArguments);
        void TraceWarning(string text);
        void TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration);
        void TraceStepSkipped();
        void TraceStepPending(BindingMatch match, object[] arguments);
        void TraceBindingError(BindingException ex);
        void TraceError(Exception ex);
        void TraceNoMatchingStepDefinition(StepArgs stepArgs);
        void TraceDuration(TimeSpan elapsed, MethodInfo methodInfo, object[] arguments);
        void TraceDuration(TimeSpan elapsed, string text);
    }

    internal class TestTracer : ITestTracer
    {
        private readonly ITraceListener traceListener;
        private readonly StepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;
        private readonly IStepFormatter stepFormatter;

        public TestTracer()
        {
            this.traceListener = ObjectContainer.TraceListener;
            this.stepDefinitionSkeletonProvider = ObjectContainer.StepDefinitionSkeletonProvider;
            this.stepFormatter = ObjectContainer.StepFormatter;
        }

        public void TraceStep(StepArgs stepArgs, bool showAdditionalArguments)
        {
            string stepText = stepFormatter.GetStepText(stepArgs);
            traceListener.WriteTestOutput(stepText.TrimEnd());
        }

        public void TraceWarning(string text)
        {
            traceListener.WriteToolOutput("warning: {0}", text);
        }

        public void TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration)
        {
            traceListener.WriteToolOutput("done: {0} ({1:F1}s)", 
                stepFormatter.GetMatchText(match, arguments), duration.TotalSeconds);
        }

        public void TraceStepSkipped()
        {
            traceListener.WriteToolOutput("skipped because of previous errors");
        }

        public void TraceStepPending(BindingMatch match, object[] arguments)
        {
            traceListener.WriteToolOutput("pending: {0}",
                stepFormatter.GetMatchText(match, arguments));
        }

        public void TraceBindingError(BindingException ex)
        {
            traceListener.WriteToolOutput("binding error: {0}", ex.Message);
        }

        public void TraceError(Exception ex)
        {
            traceListener.WriteToolOutput("error: {0}", ex.Message);
        }

        public void TraceNoMatchingStepDefinition(StepArgs stepArgs)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("No matching step definition found for the step. Use the following code to create one:");
            message.Append(
                stepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                    stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(stepArgs))
                        .Indent(StepDefinitionSkeletonProvider.CODEINDENT));

            traceListener.WriteToolOutput(message.ToString());
        }

        public void TraceDuration(TimeSpan elapsed, MethodInfo methodInfo, object[] arguments)
        {
            traceListener.WriteToolOutput("duration: {0}: {1:F1}s", 
                stepFormatter.GetMatchText(methodInfo, arguments), elapsed.TotalSeconds);
        }

        public void TraceDuration(TimeSpan elapsed, string text)
        {
            traceListener.WriteToolOutput("duration: {0}: {1:F1}s", text, elapsed.TotalSeconds);
        }
    }
}