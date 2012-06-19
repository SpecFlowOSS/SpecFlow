using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    internal class StepDefinitionSkeletonProviderCS : StepDefinitionSkeletonProviderBase
    {
        public override string GetStepDefinitionSkeleton(StepInstance stepInstance)
        {
            List<string> extraArgs = new List<string>();
            if (stepInstance.MultilineTextArgument != null)
                extraArgs.Add("string multilineText");
            if (stepInstance.TableArgument != null)
                extraArgs.Add("Table table");

            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"[{0}(@""{2}"")]
public void {1}{3}({4})
{{
    ScenarioContext.Current.Pending();
}}",
                                stepInstance.StepDefinitionType,
                                LanguageHelper.GetDefaultKeyword(stepInstance.StepContext.Language, stepInstance.StepDefinitionType).ToIdentifier(),
                                EscapeRegex(stepInstance.Text),
                                stepInstance.Text.ToIdentifier(),
                                string.Join(", ", extraArgs.ToArray())
                );
            result.AppendLine();

            return result.ToString();
        }

        public override string GetBindingClassSkeleton(string stepDefinitions)
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
    }
}