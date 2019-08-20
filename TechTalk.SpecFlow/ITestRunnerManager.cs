using System;
using System.Reflection;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public interface ITestRunnerManager : IAsyncDisposable
    {
        Assembly TestAssembly { get; }
        Assembly[] BindingAssemblies { get; }
        bool IsMultiThreaded { get; }
        Task<ITestRunner> GetTestRunnerAsync(string testClassId);
        void Initialize(Assembly testAssembly);
        Task FireTestRunEndAsync();
        Task FireTestRunStartAsync();
    }
}