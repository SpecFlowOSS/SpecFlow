using System;
using System.Linq;
using System.Collections.Generic;
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
        void TraceNoMatchingStepDefinition(StepArgs stepArgs, ProgrammingLanguage targetLanguage, List<BindingMatch> matchesWithoutScopeCheck);
        void TraceDuration(TimeSpan elapsed, MethodInfo methodInfo, object[] arguments);
        void TraceDuration(TimeSpan elapsed, string text);
    }

    internal class TestTracer : ITestTracer
    {
        private readonly ITraceListener traceListener;
        private readonly IStepFormatter stepFormatter;

        public TestTracer()
        {
            this.traceListener = ObjectContainer.TraceListener;
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

        public void TraceNoMatchingStepDefinition(StepArgs stepArgs, ProgrammingLanguage targetLanguage, List<BindingMatch> matchesWithoutScopeCheck)
        {
//            string stepDescription = stepFormatter.GetStepDescription(stepArgs);
//            return new BindingException(
//                string.Format("Multiple step definitions found, but none of them have matching scope for step '{0}': {1}",
//                    stepDescription,
//                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.MethodInfo)).ToArray())));


            IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider = ObjectContainer.StepDefinitionSkeletonProvider(targetLanguage);

            StringBuilder message = new StringBuilder();
            if (matchesWithoutScopeCheck == null || matchesWithoutScopeCheck.Count == 0)
                message.AppendLine("No matching step definition found for the step. Use the following code to create one:");
            else
            {
                string preMessage = string.Format("No matching step definition found for the step. There are matching step definitions, but none of them have matching scope for this step: {0}.", 
                    string.Join(", ", matchesWithoutScopeCheck.Select(m => stepFormatter.GetMatchText(m, null)).ToArray()));
                traceListener.WriteToolOutput(preMessage);
                message.AppendLine("Change the scope or use the following code to create a new step definition:");
            }
            message.Append(
                stepDefinitionSkeletonProvider.GetBindingClassSkeleton(
                    stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(stepArgs))
                        .Indent(StepDefinitionSkeletonProviderBase.CODEINDENT));

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