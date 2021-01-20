using System.Reflection;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestAssemblyProvider
    {
        Assembly TestAssembly { get; }

        public void RegisterTestAssembly(Assembly testAssembly);
    }
}
