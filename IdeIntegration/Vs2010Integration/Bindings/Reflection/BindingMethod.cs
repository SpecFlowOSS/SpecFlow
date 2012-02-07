using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingMethod : IBindingMethod
    {
        public IBindingType Type { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<IBindingParameter> Parameters { get; private set; }

        public BindingMethod(IBindingType type, string name, IEnumerable<IBindingParameter> parameters)
        {
            Type = type;
            Name = name;
            Parameters = parameters;
        }
    }
}