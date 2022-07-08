using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Tracing
{
    public interface ITestTracer
    {
        void TraceStep(StepInstance stepInstance, bool showAdditionalArguments);
        void TraceWarning(string text);
        void TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration);
        void TraceStepSkipped();
        void TraceStepPending(BindingMatch match, object[] arguments);
        void TraceBindingError(BindingException ex);
        void TraceError(Exception ex, TimeSpan duration);
        void TraceNoMatchingStepDefinition(StepInstance stepInstance, ProgrammingLanguage targetLanguage, CultureInfo bindingCulture, List<BindingMatch> matchesWithoutScopeCheck);
        void TraceDuration(TimeSpan elapsed, IBindingMethod method, object[] arguments);
        void TraceDuration(TimeSpan elapsed, string text);
    }

    public class TestTracer : ITestTracer
    {
        private static readonly TimeSpan MillisecondsThreshold = TimeSpan.FromMilliseconds(300);

        private readonly ITraceListener traceListener;
        private readonly IStepFormatter stepFormatter;
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;
        private readonly Configuration.SpecFlowConfiguration specFlowConfiguration;
        private readonly IColorOutputTheme colorOutputTheme;
        private readonly IColorOutputHelper colorOutputHelper;

        public TestTracer(ITraceListener traceListener, IStepFormatter stepFormatter, IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider, Configuration.SpecFlowConfiguration specFlowConfiguration, IColorOutputTheme colorOutputTheme, IColorOutputHelper colorOutputHelper)
        {
            this.traceListener = traceListener;
            this.stepFormatter = stepFormatter;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
            this.specFlowConfiguration = specFlowConfiguration;
            this.colorOutputTheme = colorOutputTheme;
            this.colorOutputHelper = colorOutputHelper;
        }

        public void TraceStep(StepInstance stepInstance, bool showAdditionalArguments)
        {
            string stepText = stepFormatter.GetStepText(stepInstance);
            traceListener.WriteTestOutput(stepText.TrimEnd());
        }

        public void TraceWarning(string text)
        {
            traceListener.WriteToolOutput("{0}: {1}", colorOutputHelper.Colorize("warning", colorOutputTheme.Warning), text);
        }

        public void TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration)
        {
            traceListener.WriteToolOutput(
                "{0}: {1} ({2:F1}s)",
                colorOutputHelper.Colorize("done", colorOutputTheme.Done),
                stepFormatter.GetMatchText(match, arguments),
                duration.TotalSeconds
            );
        }

        public void TraceStepSkipped()
        {
            traceListener.WriteToolOutput("skipped because of previous errors");
        }

        public void TraceStepPending(BindingMatch match, object[] arguments)
        {
            traceListener.WriteToolOutput("{0}: {1}",
                colorOutputHelper.Colorize("pending", colorOutputTheme.Warning),
                stepFormatter.GetMatchText(match, arguments));
        }

        public void TraceBindingError(BindingException ex)
        {
            traceListener.WriteToolOutput("{0}: {1}", colorOutputHelper.Colorize("binding error", colorOutputTheme.Error), ex.Message);
        }

        public void TraceError(Exception ex, TimeSpan duration)
        {
            WriteErrorMessage(ex.Message, duration);
            WriteLoaderExceptionsIfAny(ex, duration);
        }

        private void WriteLoaderExceptionsIfAny(Exception ex, TimeSpan duration)
        {
            switch (ex)
            {
                case TypeInitializationException typeInitializationException:
                    WriteLoaderExceptionsIfAny(typeInitializationException.InnerException, duration);
                    break;
                case ReflectionTypeLoadException reflectionTypeLoadException:
                    WriteErrorMessage("Type Loader exceptions:", duration);
                    FormatLoaderExceptions(reflectionTypeLoadException, duration);
                    break;
            }
        }

        private void FormatLoaderExceptions(ReflectionTypeLoadException reflectionTypeLoadException, TimeSpan duration)
        {
            var exceptions = reflectionTypeLoadException.LoaderExceptions
                .Select(x => x.ToString())
                .Distinct()
                .Select(x => $"LoaderException: {x}");
            foreach (var ex in exceptions)
            {
                WriteErrorMessage(ex,duration);
            }
        }

        private void WriteErrorMessage(string ex,TimeSpan duration)
        {
            traceListener.WriteToolOutput(
                "{0}: {1} ({2:F1}s)",
                colorOutputHelper.Colorize("error", colorOutputTheme.Error),
                ex,
                duration.TotalSeconds
            );
        }

        public void TraceNoMatchingStepDefinition(StepInstance stepInstance, ProgrammingLanguage targetLanguage, CultureInfo bindingCulture, List<BindingMatch> matchesWithoutScopeCheck)
        {
            StringBuilder message = new StringBuilder();
            if (matchesWithoutScopeCheck == null || matchesWithoutScopeCheck.Count == 0)
                message.AppendLine("No matching step definition found for the step. Use the following code to create one:");
            else
            {
                string preMessage = "No matching step definition found for the step. There are matching step definitions, but none of them have matching scope for this step: "
                                    + $"{string.Join(", ", matchesWithoutScopeCheck.Select(m => stepFormatter.GetMatchText(m, null)).ToArray())}.";
                traceListener.WriteToolOutput(preMessage);
                message.AppendLine("Change the scope or use the following code to create a new step definition:");
            }
            message.Append(
               stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(targetLanguage, stepInstance, specFlowConfiguration.StepDefinitionSkeletonStyle, bindingCulture)
                    .Indent(StepDefinitionSkeletonProvider.METHOD_INDENT));

            traceListener.WriteToolOutput(message.ToString());
        }

        public void TraceDuration(TimeSpan elapsed, IBindingMethod method, object[] arguments)
        {
            string matchText = stepFormatter.GetMatchText(method, arguments);
            if (elapsed > MillisecondsThreshold)
            {
                traceListener.WriteToolOutput($"duration: {matchText}: {elapsed.TotalSeconds:F1}s");
            }
            else
            {
                traceListener.WriteToolOutput($"duration: {matchText}: {elapsed.TotalMilliseconds:F1}ms");
            }
        }

        public void TraceDuration(TimeSpan elapsed, string text)
        {
            if (elapsed > MillisecondsThreshold)
            {
                traceListener.WriteToolOutput($"duration: {text}: {elapsed.TotalSeconds:F1}s");
            }
            else
            {
                traceListener.WriteToolOutput($"duration: {text}: {elapsed.TotalMilliseconds:F1}ms");
            }
        }
    }
}