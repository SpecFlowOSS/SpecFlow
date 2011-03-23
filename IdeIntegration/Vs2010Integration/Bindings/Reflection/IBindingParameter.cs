namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public interface IBindingParameter
    {
        IBindingType Type { get; }
        string ParameterName { get; }
    }
}