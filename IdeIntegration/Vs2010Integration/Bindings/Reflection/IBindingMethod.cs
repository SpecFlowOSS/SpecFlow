using System.Collections.Generic;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings.Reflection
{
    public interface IBindingMethod
    {
        IBindingType Type { get; }
        string Name { get; }
        IEnumerable<IBindingParameter> Parameters { get; }
        string ShortDisplayText { get; }
    }
}