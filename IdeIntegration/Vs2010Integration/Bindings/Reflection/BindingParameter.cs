namespace TechTalk.SpecFlow.Bindings.Reflection
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
    }
}