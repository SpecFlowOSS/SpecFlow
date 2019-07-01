using System.Diagnostics;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using global::TechTalk.SpecFlow;

[TestClass]
public class MSTestAssemblyHooks
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        var currentAssembly = typeof(MSTestAssemblyHooks).Assembly;

        TestRunnerManager.OnTestRunStart(currentAssembly);
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        var currentAssembly = typeof(MSTestAssemblyHooks).Assembly;

        TestRunnerManager.OnTestRunEnd(currentAssembly);
    }
}
