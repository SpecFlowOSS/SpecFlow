using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Tracing
{
    public abstract class StepDefinitionSkeletonProviderBase : IStepDefinitionSkeletonProvider
    {
        public const string CODEINDENT = "    ";
        public const string QuotesRegex = "\"(\")+(.*?)\"(\")+";
        public abstract string GetStepDefinitionSkeleton(StepArgs steps);
        public abstract string GetBindingClassSkeleton(List<StepArgs> steps);
        public abstract string GetFileSkeleton(List<StepArgs> steps, StepDefSkeletonInfo info);
        public abstract string AddStepsToExistingFile(string file, List<StepArgs> steps);

        protected static string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }

        protected static string GetAttributeName(Type attributeType)
        {
            return attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length);
        }

        /*This method only escapes the special characters that are not contained within quotes */
        protected static string EscapeRegexOutsideQuotes(string text)
        {
            var sections = text.Split('"');
            string newText = "";
            for (int i = 0; i < sections.Length; i++)
                if (i % 2 == 1)
                    newText += "\"\"" + sections[i] + "\"\""; //If it is in quotes, then place within double quotes and pass back
                else
                    newText += Regex.Escape(sections[i]).Replace("\\ ", " "); //If it is outside quotes then escape metacharacters
            return newText;
        }

        protected void GroupByBindingType(IEnumerable<StepArgs> steps, out List<string> givens, out List<String> whens, out List<string> thens)
        {
            givens = new List<string>();
            whens = new List<string>();
            thens = new List<string>();

            //Generate the steps skeletons and add them to the appropriate category
            foreach (StepArgs stepArgs in steps)
            {
                string stepDefSkel = GetStepDefinitionSkeleton(stepArgs);
                switch (stepArgs.Type)
                {
                    case BindingType.Given:
                        if (!givens.Contains(stepDefSkel))
                            givens.Add(stepDefSkel);
                        break;
                    case BindingType.When:
                        if (!whens.Contains(stepDefSkel))
                            whens.Add(stepDefSkel);
                        break;
                    default:
                        if (!thens.Contains(stepDefSkel))
                            thens.Add(stepDefSkel);
                        break;
                }
            }
        }

        protected string CombineMethods(IEnumerable<string> steps)
        {
            var result = new StringBuilder();
            foreach (var step in steps)
            {
                result.Append(step.Indent(CODEINDENT)).AppendLine(CODEINDENT);
            }
            return result.ToString();
        }

        protected List<string> GetCombinedMethodsSkeleton(List<StepArgs> steps)
        {
            return steps.Select(GetStepDefinitionSkeleton).ToList();
        }
    }
}
