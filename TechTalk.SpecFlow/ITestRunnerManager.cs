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
        ITestRunner GetTestRunner(string testClassId);
        void Initialize(Assembly testAssembly);
        Task FireTestRunEndAsync();
        Task FireTestRunStartAsync();
    }
}
