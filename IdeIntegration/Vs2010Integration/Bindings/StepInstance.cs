using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings
{
    public class StepInstance
    {
        public BindingType BindingType { get; private set; }
        public string Text { get; private set; }
        public StepScope StepScope { get; private set; }
        public string MultilineTextArgument { get; set; }
        public Table TableArgument { get; set; }

        public StepInstance(BindingType bindingType, string stepText, StepScope stepScope)
        {
            BindingType = bindingType;
            Text = stepText;
            StepScope = stepScope;
        }
    }
}
