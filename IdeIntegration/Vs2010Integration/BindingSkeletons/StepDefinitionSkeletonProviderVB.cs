using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    internal class StepDefinitionSkeletonProviderVB : StepDefinitionSkeletonProviderBase
    {
        public StepDefinitionSkeletonProviderVB(GherkinDialect gherkinDialect) : base(gherkinDialect)
        {
        }

        public override string GetStepDefinitionSkeleton(StepInstance stepInstance)
        {
            List<string> extraArgs = new List<string>();
            if (stepInstance.MultilineTextArgument != null)
                extraArgs.Add("ByVal multilineText As String");
            if (stepInstance.TableArgument != null)
                extraArgs.Add("ByVal table As Table");

            StringBuilder result = new StringBuilder();

            // in VB "When" and "Then" are language keywords - use the fully qualified namespace
            string namespaceAddition = "TechTalk.SpecFlow.";
            if (stepInstance.BindingType == BindingType.Given)
            {
                namespaceAddition = "";
            }

            result.AppendFormat(@"<{4}{0}(""{1}"")> _
Public Sub {2}({3})
    ScenarioContext.Current.Pending()
End Sub",
                stepInstance.BindingType,
                EscapeRegex(stepInstance.Text),
                GetStepText(stepInstance).ToIdentifier(),
                string.Join(", ", extraArgs.ToArray()),
                namespaceAddition
                );
            result.AppendLine();

            return result.ToString();
        }

        public override string GetBindingClassSkeleton(string stepDefinitions)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"<{0}> _
Public Class StepDefinitions

{1}
End Class",
                "Binding", //TODO GetAttributeName(typeof(BindingAttribute)),
                stepDefinitions.Indent(CODEINDENT));
            result.AppendLine();

            return result.ToString();
        }

    }

}