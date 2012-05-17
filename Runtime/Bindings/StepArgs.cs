using System;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepArgs
    {
        public StepDefinitionType StepDefinitionType { get; private set; }
        public StepDefinitionKeyword StepDefinitionKeyword { get; private set; }
        public string Keyword { get; private set; } //TODO: we should preserve the original keyword as well

        public string Text { get; private set; }
        public string MultilineTextArgument { get; private set; }
        public Table TableArgument { get; private set; }

        public StepContext StepContext { get; private set; }
      
        public StepArgs(StepDefinitionType type, StepDefinitionKeyword stepDefinitionKeyword, string text, string multilineTextArgument, Table tableArgument, StepContext stepContext)
        {
            StepDefinitionType = type;
            StepDefinitionKeyword = stepDefinitionKeyword;
            Text = text;
            MultilineTextArgument = multilineTextArgument;
            TableArgument = tableArgument;
            StepContext = stepContext;
        }
    }
}