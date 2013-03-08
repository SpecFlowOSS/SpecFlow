using System.Reflection;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IBindingAssemblyLoader
    {
        Assembly Load(string assemblyName);
    }

    public class BindingAssemblyLoader : IBindingAssemblyLoader
    {
        public Assembly Load(string assemblyName)
        {
#if WINRT
            return Assembly.Load(new AssemblyName(assemblyName));
#else
            return Assembly.Load(assemblyName);
#endif
        }
    }
}