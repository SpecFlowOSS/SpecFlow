using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons
{
    public class StepDefinitionSkeletonProviderVB : StepDefinitionSkeletonProviderBase
    {
        public override string GetStepDefinitionSkeleton(StepInstance stepInfo)
        {
            List<string> extraArgs = new List<string>();

            if (stepInfo.MultilineTextArgument != null)
                extraArgs.Add("ByVal multilineText As String");
            if (stepInfo.TableArgument != null)
                extraArgs.Add("ByVal table As Table");

            string stepText = EscapeRegexOutsideQuotes(stepInfo.Text);
            extraArgs.AddRange(ParseArgsFromQuotes(ref stepText));
            //Adds parameters passed in via quotes to the args for the method
            string methodName = Regex.Replace(EscapeRegex(stepInfo.Text), QuotesRegex, "").ToIdentifier();
            StringBuilder result = new StringBuilder();

            // in VB "When" and "Then" are language keywords - use the fully qualified namespace
            string namespaceAddition = "TechTalk.SpecFlow.";
            if (stepInfo.BindingType == BindingType.Given)
            {
                namespaceAddition = "";
            }

            result.AppendFormat(
@"<{4}{0}(""{1}"")> _
Public Sub {0}{2}({3})
{5}ScenarioContext.Current.Pending()
End Sub",
                stepInfo.BindingType,
                stepText,
                methodName,
                string.Join(", ", extraArgs.ToArray()),
                namespaceAddition,
                CODEINDENT
                );
            result.AppendLine();

            return result.ToString();
        }

        public override string GetBindingClassSkeleton(List<StepInstance> steps)
        {
            string body = CombineMethods(GetCombinedMethodsSkeleton(steps));

            StringBuilder result = new StringBuilder();
            result.AppendFormat(@"<{0}> _
Public Class StepDefinitions
{2}
{1}End Class",
                                GetAttributeName(typeof (BindingAttribute)),
                                body,
                                CODEINDENT);
            result.AppendLine();

            return result.ToString();
        }

        public override string GetFileSkeleton(List<StepInstance> steps, StepDefSkeletonInfo info)
        {
            string body = CombineMethods(GetCombinedMethodsSkeleton(steps));
            StringBuilder result = new StringBuilder();
            result.AppendFormat(
@"Imports TechTalk.SpecFlow

<{0}> _
Public Class {2}
{3}
{1}End Class",
                                GetAttributeName(typeof (BindingAttribute)),
                                body,
                                info.SuggestedStepDefName,
                                CODEINDENT);
            result.AppendLine();
            return result.ToString();
        }

        public override string AddStepsToExistingFile(string file, List<StepInstance> steps)
        {
            string body = CombineMethods(GetCombinedMethodsSkeleton(steps));
            if (!string.IsNullOrEmpty(body))
                if (!TryAddRemainingSteps(ref file, body))
                    return null;
            return file;
        }

        /// <summary>
        /// Attemps to add steps to the top of a binding class file.
        /// </summary>
        private bool TryAddRemainingSteps(ref string file, string body)
        {
            int posBinding = file.IndexOf("<Binding");
            if (posBinding != -1)
            {
                int posClass = file.IndexOf("Class", posBinding);
                if (posClass != -1)
                {
                    int startPos = file.IndexOf("End Class", posClass);
                    if (startPos != -1)
                    {
                        file = file.Substring(0, startPos) + body +
                            file.Substring(startPos);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Takes anything between quotes in a string and parses its type to store it as a parameter for the method.
        /// </summary>
        private IEnumerable<string> ParseArgsFromQuotes(ref string text)
        {
            var args = new List<string>();
            int i = 1;
            int offset = 0; //when text is replaced the change in length must be recorded to update the indices

            Regex matchText = new Regex(QuotesRegex);
            Match match = matchText.Match(text);

            while (match.Success)
            {
                string type, replacementRegex;

                int paramPos = match.Index + 2 + offset; //starting position of the regex, +2 to avoid the quotes, +offset to account for the changes of previous replacements
                offset -= text.Length;
                string param = match.ToString().Substring(2, match.ToString().Length - 4); //get the match string without the quotes

                if (param.CanParseInt())
                {
                    type = "Integer";
                    replacementRegex = "(\\d+)";
                }
                else if (param.CanParseBool())
                {
                    type = "Boolean";
                    replacementRegex = "(True|False)";
                }
                else
                {
                    if (param.CanParseDouble())
                        type = "Double";
                    else if (param.CanParseDateTime())
                        type = "Date";
                    else
                        type = "String";
                    replacementRegex = "(.*)";
                }
                string paramName = String.Format("{0}{1}", type[0].ToString().ToLower(), type.Substring(1));
                args.Add("ByVal " + paramName + i++ + " As " + type);
                text = text.Substring(0, paramPos) + replacementRegex + text.Substring(paramPos + param.Length);
                offset += text.Length;
                match = match.NextMatch();
            }
            return args;
        }
    }
}