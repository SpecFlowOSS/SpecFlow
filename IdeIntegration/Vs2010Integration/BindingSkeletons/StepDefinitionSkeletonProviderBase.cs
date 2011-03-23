using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;
using System.Linq;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    internal abstract class StepDefinitionSkeletonProviderBase : IStepDefinitionSkeletonProvider
    {
        public const string CODEINDENT = "    ";

        private GherkinDialect gherkinDialect;

        protected StepDefinitionSkeletonProviderBase(GherkinDialect gherkinDialect)
        {
            this.gherkinDialect = gherkinDialect;
        }

        public abstract string GetStepDefinitionSkeleton(StepInstance stepInstance);
        public abstract string GetBindingClassSkeleton(string stepDefinitions);

        protected static string GetAttributeName(Type attributeType)
        {
            return attributeType.Name.Substring(0, attributeType.Name.Length - "Attribute".Length);
        }

        protected static string EscapeRegex(string text)
        {
            return Regex.Escape(text).Replace("\"", "\"\"").Replace("\\ ", " ");
        }

        protected string GetStepText(StepInstance stepInstance)
        {
            string keyword;

            if (stepInstance.StepDefinitionKeyword == StepDefinitionKeyword.Given ||
                stepInstance.StepDefinitionKeyword == StepDefinitionKeyword.When ||
                stepInstance.StepDefinitionKeyword == StepDefinitionKeyword.Then)
                keyword = stepInstance.Keyword;
            else
                keyword = gherkinDialect.GetStepKeywords((StepKeyword) stepInstance.BindingType).FirstOrDefault(k => !k.StartsWith("*")) ?? "";

            return keyword + stepInstance.Text;
        }
    }
}