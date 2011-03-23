using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepInstance
    {
        public BindingType BindingType { get; private set; }
        public StepDefinitionKeyword StepDefinitionKeyword { get; private set; }
        public string Keyword { get; private set; }
        public string Text { get; private set; }
        public StepScope StepScope { get; private set; }
        public string MultilineTextArgument { get; set; }
        public Table TableArgument { get; set; }

        public StepInstance(BindingType bindingType, StepDefinitionKeyword stepDefinitionKeyword, string keyword, string stepText, StepScope stepScope)
        {
            BindingType = bindingType;
            StepDefinitionKeyword = stepDefinitionKeyword;
            Keyword = keyword;
            Text = stepText;
            StepScope = stepScope;
        }
    }
}
