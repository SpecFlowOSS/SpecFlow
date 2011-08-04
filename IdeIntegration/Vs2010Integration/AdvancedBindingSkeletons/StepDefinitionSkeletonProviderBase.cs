using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.AdvancedBindingSkeletons
{
    public abstract class StepDefinitionSkeletonProviderBase : IStepDefinitionSkeletonProvider
    {
        public const string CODEINDENT = "    ";
        public const string QuotesRegex = "\"(\")+(.*?)\"(\")+";
        public abstract string GetStepDefinitionSkeleton(StepInstance steps);
        public abstract string GetBindingClassSkeleton(List<StepInstance> steps);
        public abstract string GetFileSkeleton(List<StepInstance> steps, StepDefSkeletonInfo info);
        public abstract string AddStepsToExistingFile(string file, List<StepInstance> steps);

        protected static string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }

        protected static string GetAttributeName(Type attributeType)
        {
            return attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length);
        }

        /// <summary>
        /// Escapes the special characters that are not contained within quotes.
        /// </summary>
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

        /// <summary>
        /// Splits a list of steps into 3 seperate lists according to the binding type of each step.
        /// </summary>
        protected void GroupByBindingType(IEnumerable<StepInstance> steps, out List<string> givens, out List<String> whens, out List<string> thens)
        {
            givens = new List<string>();
            whens = new List<string>();
            thens = new List<string>();

            //Generate the steps skeletons and add them to the appropriate category
            foreach (StepInstance stepArgs in steps)
            {
                string stepDefSkel = GetStepDefinitionSkeleton(stepArgs);
                switch (stepArgs.BindingType)
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

        /// <summary>
        /// Takes a list of method declarations and combines them to produce the body of a class.
        /// </summary>
        protected string CombineMethods(IEnumerable<string> steps)
        {
            var result = new StringBuilder();
            foreach (var step in steps)
            {
                result.Append(step.Indent(CODEINDENT)).AppendLine(CODEINDENT);
            }
            return result.ToString();
        }

        /// <summary>
        /// Turns each step in the list into a string representing its suggested step definition.
        /// </summary>
        protected List<string> GetCombinedMethodsSkeleton(List<StepInstance> steps)
        {
            return steps.Select(GetStepDefinitionSkeleton).ToList();
        }
    }
}
