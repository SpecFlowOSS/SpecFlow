using System;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepInstance
    {
        public StepDefinitionType StepDefinitionType { get; private set; }
        public StepDefinitionKeyword StepDefinitionKeyword { get; private set; }
        /// <summary>
        /// Keyword (including trailing spaces) used in the original specs or <value>null</value> if unknown.
        /// </summary>
        public string Keyword { get; private set; }

        public string Text { get; private set; }
        public string MultilineTextArgument { get; set; }
        public Table TableArgument { get; set; }

        public StepContext StepContext { get; private set; }

        public StepInstance(StepDefinitionType stepDefinitionType, StepDefinitionKeyword stepDefinitionKeyword, string keywordWithTrailingSpaces, string text, StepContext stepContext)
                : this(stepDefinitionType, stepDefinitionKeyword, keywordWithTrailingSpaces, text, null, null, stepContext)
        {
        }

        public StepInstance(StepDefinitionType type, StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArgument, Table tableArgument, StepContext stepContext)
        {
            StepDefinitionType = type;
            StepDefinitionKeyword = stepDefinitionKeyword;
            Text = text;
            MultilineTextArgument = multilineTextArgument;
            TableArgument = tableArgument;
            StepContext = stepContext;
            Keyword = keyword;
        }
    }
}