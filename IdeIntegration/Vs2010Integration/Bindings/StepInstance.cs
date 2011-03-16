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

        public StepInstance(BindingType bindingType, string stepText)
        {
            BindingType = bindingType;
            Text = stepText;
        }

        public bool Match(StepBinding binding, bool includeRegexCheck)
        {
            if (includeRegexCheck && binding.Regex != null && !binding.Regex.Match(Text).Success)
                return false;

            //TODO: consider scope & params
            return BindingType == binding.BindingType;
        }
    }
}
