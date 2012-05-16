using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public interface IBindingMethod
    {
        IBindingType Type { get; }
        string Name { get; }
        IEnumerable<IBindingParameter> Parameters { get; }
    }
}