using System;
using System.Linq;

namespace TechTalk.SpecFlow
{
    internal class StepArgs
    {
        public StepDefinitionKeyword StepDefinitionKeyword { get; private set; }
        //TODO: we should preserve the original keyword as well
        public string OriginalStepKeyword;
        public BindingType Type { get; private set; }
        public string Text { get; private set; }
        public string MultilineTextArgument { get; private set; }
        public Table TableArgument { get; private set; }

        public StepArgs(BindingType type, StepDefinitionKeyword stepDefinitionKeyword, string text, string multilineTextArgument, Table tableArgument)
        {
            Type = type;
            StepDefinitionKeyword = stepDefinitionKeyword;
            Text = text;
            MultilineTextArgument = multilineTextArgument;
            TableArgument = tableArgument;
        }
    }
}