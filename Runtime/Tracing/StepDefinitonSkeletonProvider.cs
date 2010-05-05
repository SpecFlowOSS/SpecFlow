using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    internal class StepDefinitionSkeletonProvider
    {
        public const string CODEINDENT = "    ";

        public string GetStepDefinitionSkeleton(StepArgs stepArgs)
        {
            List<string> extraArgs = new List<string>();
            if (stepArgs.MultilineTextArgument != null)
                extraArgs.Add("string multilineText");
            if (stepArgs.TableArgument != null)
                extraArgs.Add("Table table");

            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"[{0}(@""{2}"")]
public void {1}{3}({4})
{{
    ScenarioContext.Current.Pending();
}}",
                stepArgs.Type,
                LanguageHelper.GetDefaultKeyword(FeatureContext.Current.FeatureInfo.Language, stepArgs.Type).ToIdentifier(),
                EscapeRegex(stepArgs.Text),
                stepArgs.Text.ToIdentifier(),
                string.Join(", ", extraArgs.ToArray())
                );
            result.AppendLine();

            return result.ToString();
        }

        public string GetBindingClassSkeleton(string stepDefinitions)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"[{0}]
public class StepDefinitions
{{
{1}}}",
                GetAttributeName(typeof(BindingAttribute)),
                stepDefinitions.Indent(CODEINDENT));
            result.AppendLine();

            return result.ToString();
        }

        private string GetAttributeName(Type attributeType)
        {
            return attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length);
        }

        private string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }
    }
}