using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public interface IRuntimeBindingRegistryBuilder
    {
        void BuildBindingsFromAssembly(Assembly assembly);
    }
}