using System.Diagnostics;
using System.CodeDom.Compiler;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using global::TechTalk.SpecFlow;

[GeneratedCode("SpecFlow", "SPECFLOW_VERSION")]
[TestClass]
public class PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        var currentAssembly = typeof(PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks).Assembly;

        TestRunnerManager.OnTestRunStart(currentAssembly);
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        var currentAssembly = typeof(PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks).Assembly;

        TestRunnerManager.OnTestRunEnd(currentAssembly);
    }
}
