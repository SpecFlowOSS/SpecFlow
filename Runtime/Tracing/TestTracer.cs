using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Tracing
{
    public abstract class TestTracer
    {
        public void Warning(string text)
        {
            TraceLine("Warning: {0}", text);
        }

        private string GetStepDescription(StepArgs stepArgs)
        {
            return string.Format("{0} {1}", stepArgs.Type, stepArgs.Text);
        }

        private string GetParamString(object arg)
        {
            const int maxLength = 20;

            if (arg == null)
                return "null";

            if (arg is string)
                return "\"" + arg.ToString().Replace(Environment.NewLine, @"\r\n").TrimEllipse(maxLength) + "\"";

            if (arg is Table)
                return "<table>";

            return arg.ToString().TrimEllipse(maxLength);
        }

        internal string GetMatchText(BindingMatch match, object[] arguments)
        {
            var methodInfo = match.StepBinding.MethodInfo;
            return GetMatchText(methodInfo, arguments);
        }

        internal string GetMatchText(MethodInfo methodInfo, object[] arguments)
        {
            string argText = arguments == null ? "" : string.Join(", ", arguments.Select(a => GetParamString(a)).ToArray());
            return string.Format("{0}.{1}({2})", methodInfo.DeclaringType.Name, methodInfo.Name, argText);
        }

        internal void TraceStep(StepArgs stepArgs, bool showAdditionalArguments)
        {
            StringBuilder result = new StringBuilder();
            result.Append(stepArgs.StepDefinitionKeyword.ToString());
            result.Append(" ");
            result.AppendLine(stepArgs.Text);

            if (showAdditionalArguments)
            {
                if (stepArgs.MultilineTextArgument != null)
                {
                    result.AppendLine("--- multiline step argument ---".Indent("  "));
                    result.AppendLine(stepArgs.MultilineTextArgument.Indent("  "));
                }

                if (stepArgs.TableArgument != null)
                {
                    result.AppendLine("--- table step argument ---".Indent("  "));
                    result.AppendLine(stepArgs.TableArgument.ToString().Indent("  "));
                }
            }

            Trace(result.ToString());
        }

        internal string GetMethodText(MethodInfo methodInfo)
        {
            return string.Format("{0}.{1}({2})", methodInfo.DeclaringType.Name, methodInfo.Name,
                string.Join(", ", methodInfo.GetParameters().Select(pi => pi.ParameterType.Name).ToArray()));
        }

        internal Exception GetCallError(MethodInfo methodInfo, Exception ex)
        {
            return new BindingException(
                string.Format("Error calling binding method '{0}': {1}",
                    GetMethodText(methodInfo), ex.Message));
        }

        internal Exception GetParameterCountError(BindingMatch match, int expectedParameterCount)
        {
            return new BindingException(
                string.Format("Parameter count mismatch! The binding method '{0}' should have {1} parameters",
                    GetMethodText(match.StepBinding.MethodInfo), expectedParameterCount));
        }

        internal Exception GetAmbiguousMatchError(IEnumerable<BindingMatch> matches, StepArgs stepArgs)
        {
            string stepDescription = GetStepDescription(stepArgs);
            return new BindingException(
                string.Format("Ambiguous step definitions found for step '{0}': {1}",
                    stepDescription,
                    string.Join(", ", matches.Select(m => GetMethodText(m.StepBinding.MethodInfo)).ToArray())));
        }

        private string GetAttributeName(Type attributeType)
        {
            return attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length);
        }

        internal string GetStepDefinitionSkeleton(StepArgs stepArgs)
        {
            List<string> extraArgs = new List<string>();
            if (stepArgs.MultilineTextArgument != null)
                extraArgs.Add("string multilineText");
            if (stepArgs.TableArgument != null)
                extraArgs.Add("Table table");

            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"[{0}(@""{1}"")]
public void {0}{2}({3})
{{
    ScenarioContext.Current.Pending();
}}",
                stepArgs.Type,
                EscapeRegex(stepArgs.Text),
                stepArgs.Text.ToIdentifier(),
                string.Join(", ", extraArgs.ToArray())
                );
            result.AppendLine();

            return result.ToString();
        }

        internal string GetBindingClassSkeleton(string stepDefinitions)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"[{0}]
public class StepDefinitons
{{
{1}}}",
                GetAttributeName(typeof(BindingAttribute)),
                stepDefinitions.Indent("    "));
            result.AppendLine();

            return result.ToString();
        }

        internal void NoMatchingStepDefinition(StepArgs stepArgs)
        {
            TraceLine("-> No matching step definition found for the step. Use the following code to create one:");
            TraceLine(GetBindingClassSkeleton(GetStepDefinitionSkeleton(stepArgs)).Indent("    "));
        }

        private string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }


        public void TraceLine(string text)
        {
            Trace(text + Environment.NewLine);
        }

        public void TraceLine(string text, params object[] args)
        {
            Trace(text + Environment.NewLine, args);
        }

        public void Trace(string text, params object[] args)
        {
            Trace(string.Format(CultureInfo.InvariantCulture, text, args));
        }
        public abstract void Trace(string text);

        public void TraceDuration(TimeSpan elapsed, MethodInfo methodInfo, object[] arguments)
        {
            TraceLine("-> duration: {0}: {1:F1}s", GetMatchText(methodInfo, arguments), elapsed.TotalSeconds);
        }

        public void TraceDuration(TimeSpan elapsed, string text)
        {
            TraceLine("-> duration: {0}: {1:F1}s", text, elapsed.TotalSeconds);
        }
    }

    public class ConsoleTestTracer : TestTracer
    {
        public override void Trace(string text)
        {
            Console.Write(text);
        }
    }
}