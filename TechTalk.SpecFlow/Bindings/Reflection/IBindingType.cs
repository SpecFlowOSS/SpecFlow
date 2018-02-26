
namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public interface IBindingType
    {
        string Name { get; }
        string FullName { get; }
        string AssemblyName { get; }
    }
}