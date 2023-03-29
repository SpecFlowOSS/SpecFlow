using System.Reflection;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestRunnerFactory
    {
        ITestRunner Create(Assembly testAssembly);
    }
}
