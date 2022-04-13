using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow
{
    public interface ITestRunnerManager : IAsyncDisposable
    {
        Assembly TestAssembly { get; }
        Assembly[] BindingAssemblies { get; }
        bool IsMultiThreaded { get; }
        ITestRunner GetTestRunner(string workerId);
        void Initialize(Assembly testAssembly);
        Task FireTestRunEndAsync();
        Task FireTestRunStartAsync();
    }
}
