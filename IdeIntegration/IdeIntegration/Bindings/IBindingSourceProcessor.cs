using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    public interface IBindingSourceProcessor
    {
        bool CanProcessTypeAttribute(string attributeTypeName);
        bool CanProcessMethodAttribute(string attributeTypeName);

        bool PreFilterType(IEnumerable<string> attributeTypeNames);

        bool ProcessType(BindingSourceType bindingSourceType);
        void ProcessMethod(BindingSourceMethod bindingSourceMethod);
    }
}