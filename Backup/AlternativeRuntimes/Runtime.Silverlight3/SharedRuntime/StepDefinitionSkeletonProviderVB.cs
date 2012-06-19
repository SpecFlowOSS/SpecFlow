using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Tracing
{
    internal class StepDefinitionSkeletonProviderVB : StepDefinitionSkeletonProviderBase
    {
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
            if (stepInstance.StepDefinitionType == StepDefinitionType.Given)
            {
                namespaceAddition = "";
            }

            result.AppendFormat(@"<{5}{0}(""{2}"")> _
Public Sub {1}{3}({4})
    ScenarioContext.Current.Pending()
End Sub",
                stepInstance.StepDefinitionType,
                LanguageHelper.GetDefaultKeyword(stepInstance.StepContext.Language, stepInstance.StepDefinitionType).ToIdentifier(),
                EscapeRegex(stepInstance.Text),
                stepInstance.Text.ToIdentifier(),
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
                GetAttributeName(typeof(BindingAttribute)),
                stepDefinitions.Indent(CODEINDENT));
            result.AppendLine();

            return result.ToString();
        }

    }

}