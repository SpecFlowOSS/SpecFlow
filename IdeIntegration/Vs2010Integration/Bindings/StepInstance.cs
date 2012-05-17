using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepInstance
    {
        public StepDefinitionType StepDefinitionType { get; private set; }
        public StepDefinitionKeyword StepDefinitionKeyword { get; private set; }
        public string Keyword { get; private set; }

        public string Text { get; private set; }
        public string MultilineTextArgument { get; set; }
        public Table TableArgument { get; set; }

        public StepContext StepContext { get; private set; }

        public StepInstance(StepDefinitionType stepDefinitionType, StepDefinitionKeyword stepDefinitionKeyword, string keyword, string stepText, StepContext stepContext)
        {
            StepDefinitionType = stepDefinitionType;
            StepDefinitionKeyword = stepDefinitionKeyword;
            Keyword = keyword;
            Text = stepText;
            StepContext = stepContext;
        }
    }
}
