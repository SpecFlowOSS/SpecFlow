using System.Diagnostics;
using global::NUnit.Framework;
using global::TechTalk.SpecFlow;
using System.Threading.Tasks;

[SetUpFixture]
public class NUnitAssemblyHooks
{
    [OneTimeSetUp]
    public async Task AssemblyInitializeAsync()
    {
        var currentAssembly = typeof(NUnitAssemblyHooks).Assembly;

        await TestRunnerManager.OnTestRunStartAsync(nameof(NUnitAssemblyHooks), currentAssembly);
    }

    [OneTimeTearDown]
    public async Task AssemblyCleanupAsync()
    {
        var currentAssembly = typeof(NUnitAssemblyHooks).Assembly;

        await TestRunnerManager.OnTestRunEndAsync(currentAssembly);
    }
}
