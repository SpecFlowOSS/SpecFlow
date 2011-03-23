using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    internal class StepDefinitionSkeletonProviderCS : StepDefinitionSkeletonProviderBase
    {
        public StepDefinitionSkeletonProviderCS(GherkinDialect gherkinDialect) : base(gherkinDialect)
        {
        }

        public override string GetStepDefinitionSkeleton(StepInstance stepInstance)
        {
            List<string> extraArgs = new List<string>();
            if (stepInstance.MultilineTextArgument != null)
                extraArgs.Add("string multilineText");
            if (stepInstance.TableArgument != null)
                extraArgs.Add("Table table");

            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"[{0}(@""{1}"")]
public void {2}({3})
{{
    ScenarioContext.Current.Pending();
}}",
                                stepInstance.BindingType,
                                EscapeRegex(stepInstance.Text),
                                GetStepText(stepInstance).ToIdentifier(),
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
                                "Binding", //TODO GetAttributeName(typeof(BindingAttribute)),
                                stepDefinitions.Indent(CODEINDENT));
            result.AppendLine();

            return result.ToString();
        }
    }
}