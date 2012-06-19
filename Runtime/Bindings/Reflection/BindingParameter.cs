namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingParameter : IBindingParameter
    {
        public IBindingType Type { get; private set; }
        public string ParameterName { get; private set; }
        public bool IsParamArray { get; private set; }

        public BindingParameter(IBindingType type, string parameterName) : this(type, parameterName, false) { }

        public BindingParameter(IBindingType type, string parameterName, bool isParamArray)
        {
            Type = type;
            ParameterName = parameterName;
            IsParamArray = isParamArray;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ParameterName, Type);
        }

    }
}