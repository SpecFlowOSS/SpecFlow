using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingMethod : IBindingMethod
    {
        public IBindingType Type { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<IBindingParameter> Parameters { get; private set; }
        public IBindingType ReturnType { get; private set; }

        public BindingMethod(IBindingType type, string name, IEnumerable<IBindingParameter> parameters, IBindingType returnType)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            if (parameters == null) throw new ArgumentNullException("parameters");

            Type = type;
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return this.GetShortDisplayText();
        }
    }
}