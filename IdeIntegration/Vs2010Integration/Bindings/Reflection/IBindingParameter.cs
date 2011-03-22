namespace TechTalk.SpecFlow.Vs2010Integration.Bindings.Reflection
{
    public interface IBindingParameter
    {
        IBindingType Type { get; }
        string ParameterName { get; }
    }
}