using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons
{
    public class StepDefinitionSkeletonProviderCS : StepDefinitionSkeletonProviderBase
    {
        public override string GetStepDefinitionSkeleton(StepInstance stepArgs)
        {
            List<string> extraArgs = new List<string>();

            if (stepArgs.MultilineTextArgument != null)
                extraArgs.Add("string multilineText");
            if (stepArgs.TableArgument != null)
                extraArgs.Add("Table table");

            string stepText = EscapeRegexOutsideQuotes(stepArgs.Text);
            string methodName = Regex.Replace(EscapeRegex(stepArgs.Text), QuotesRegex, "").ToIdentifier();

            extraArgs.AddRange(ParseArgsFromQuotes(ref stepText)); //Adds values passed in via quotes to the args for the method

            StringBuilder result = new StringBuilder();
            result.AppendFormat(
@"[{0}(@""{1}"")]
public void {0}{2}({3})
{{
    ScenarioContext.Current.Pending();
}}",
                stepArgs.BindingType,
                stepText,
                methodName,
                string.Join(", ", extraArgs.ToArray()));
            result.AppendLine();

            return result.ToString();
        }

        public override string GetBindingClassSkeleton(List<StepInstance> steps)
        {
            List<string> givens, whens, thens;
            GroupByBindingType(steps, out givens, out whens, out thens);
            string body = GetStructuredMethodsSkeleton(givens, whens, thens);

            StringBuilder result = new StringBuilder();
            result.AppendFormat(
@"[{0}]
public class StepDefinitions
{{
{1}{2}
}}",
                GetAttributeName(typeof(BindingAttribute)),
                body,
                CODEINDENT);
            result.AppendLine();
            return result.ToString();
        }

        public override string GetFileSkeleton(List<StepInstance> steps, StepDefSkeletonInfo info)
        {
            List<string> givens, whens, thens;
            GroupByBindingType(steps, out givens, out whens, out thens);
            string body = GetStructuredMethodsSkeleton(givens, whens, thens).Indent(CODEINDENT);

            StringBuilder result = new StringBuilder();
            result.AppendFormat(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace {0}
{{
{4}[{1}]
{4}public class {2}
{4}{{
{3}{4}{4}
{4}}}
}}",
              info.NamespaceName,
              GetAttributeName(typeof(BindingAttribute)),
              info.SuggestedStepDefName,
              body,
              CODEINDENT);
            result.AppendLine();
            return result.ToString();
        }

        public override string AddStepsToExistingFile(string file, List<StepInstance> steps)
        {
            //Seperates the steps into groups of skeletons according to their binding type
            List<string> givens, whens, thens;
            GroupByBindingType(steps, out givens, out whens, out thens);

            //Try to add the steps into regions, if its succeeds the lists are cleared so that they are not added later
            if (TryAddToRegion(ref file, givens, "Given"))
                givens.RemoveRange(0, givens.Count);
            if (TryAddToRegion(ref file, whens, "When"))
                whens.RemoveRange(0, whens.Count);
            if (TryAddToRegion(ref file, thens, "Then"))
                thens.RemoveRange(0, thens.Count);

            //The steps that weren't added succesfully to regions are added to the file
            string body = GetStructuredMethodsSkeleton(givens, whens, thens);
            if (!string.IsNullOrEmpty(body))
            {
                body = body.TrimEnd(Environment.NewLine.ToCharArray()).Indent(CODEINDENT);
                if (!TryAddRemainingSteps(ref file, body))
                    return null;
            }
            return file;
        }

        /// <summary>
        /// Attempts to add steps to an existing file inside regions already set up in that file.
        /// </summary>
        private bool TryAddToRegion(ref string file, List<string> steps, string regionName)
        {
            if (steps == null || steps.Count() == 0)
                return false;
            string regionString = "#region " + regionName + Environment.NewLine;
            string body = CombineMethods(steps);

            /*The last two lines are removed and replaced with a single line,
            this ensures the correct spacing and indentations of the added steps*/
            body = body.Remove(body.LastIndexOf(Environment.NewLine));
            body = body.Remove(body.LastIndexOf(Environment.NewLine));
            body += Environment.NewLine;

            body = body.Indent(CODEINDENT);
            int insertPos = file.IndexOf(regionString);
            if (insertPos == -1)
                return false;
            var result = new StringBuilder();
            result.AppendFormat(
@"{0}{3}{3}
{1}{2}", file.Substring(0, insertPos + regionString.Length), body, file.Substring(insertPos + regionString.Length), CODEINDENT);
            file = result.ToString();
            return true;
        }

        /// <summary>
        /// Attemps to add steps to the top of a binding class file.
        /// </summary>
        private bool TryAddRemainingSteps(ref string file, string body)
        {
            int posBinding = file.IndexOf("[Binding");
            if (posBinding != -1)
            {
                int posClass = file.IndexOf("class", posBinding);
                if (posClass != -1)
                {
                    int startPos = file.IndexOf("{", posClass);
                    if (startPos != -1)
                    {
                        var result = new StringBuilder();
                        result.AppendFormat(
@"{0}
{1}{2}", file.Substring(0, startPos + 1), body, file.Substring(startPos + 1));
                        file = result.ToString();
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
                    type = "int";
                    replacementRegex = "(\\d+)";
                }
                else if (param.CanParseBool())
                {
                    type = "bool";
                    replacementRegex = "(True|False)";
                }
                else
                {
                    if (param.CanParseDouble())
                        type = "double";
                    else if (param.CanParseDateTime())
                        type = "DateTime";
                    else
                        type = "string";
                    replacementRegex = "(.*)";
                }
                string paramName = String.Format("{0}{1}", type[0].ToString().ToLower(), type.Substring(1));
                args.Add(type + " " + paramName + i++);
                text = text.Substring(0, paramPos) + replacementRegex + text.Substring(paramPos + param.Length);
                offset += text.Length;
                match = match.NextMatch();
            }
            return args;
        }

        /// <summary>
        /// Takes 3 sets of steps and combines them into a structured skeleton string.
        /// </summary>
        private string GetStructuredMethodsSkeleton(List<string> givens, List<string> whens, List<string> thens)
        {
            StringBuilder result = new StringBuilder();
            if (givens.Count() > 0)
            {
                result.AppendLine(CODEINDENT).AppendFormat(
@"{0}#region Given
{0}
{1}{0}#endregion", CODEINDENT, CombineMethods(givens)).AppendLine();
            }
            if (whens.Count() > 0)
            {
                result.AppendLine(CODEINDENT).AppendFormat(
@"{0}#region When
{0}
{1}{0}#endregion", CODEINDENT, CombineMethods(whens)).AppendLine();
            }
            if (thens.Count() > 0)
            {
                result.AppendLine(CODEINDENT).AppendFormat(
@"{0}#region Then
{0}
{1}{0}#endregion", CODEINDENT, CombineMethods(thens)).AppendLine();
            }
            return result.ToString();
        }
    }
}
