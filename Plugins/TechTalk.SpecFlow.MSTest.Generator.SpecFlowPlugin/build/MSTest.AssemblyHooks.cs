using System.Diagnostics;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using global::TechTalk.SpecFlow;
using System.Threading.Tasks;

[TestClass]
public class MSTestAssemblyHooks
{
    [AssemblyInitialize]
    public static async Task AssemblyInitializeAsync(TestContext testContext)
    {
        var currentAssembly = typeof(MSTestAssemblyHooks).Assembly;

        await TestRunnerManager.OnTestRunStartAsync(nameof(MSTestAssemblyHooks), currentAssembly);
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanupAsync()
    {
        var currentAssembly = typeof(MSTestAssemblyHooks).Assembly;

        await TestRunnerManager.OnTestRunEndAsync(currentAssembly);
    }
}
