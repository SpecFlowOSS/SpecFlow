﻿namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingParameter : IBindingParameter
    {
        public IBindingType Type { get; private set; }
        public string ParameterName { get; private set; }

        public BindingParameter(IBindingType type, string parameterName)
        {
            Type = type;
            ParameterName = parameterName;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ParameterName, Type);
        }
    }
}