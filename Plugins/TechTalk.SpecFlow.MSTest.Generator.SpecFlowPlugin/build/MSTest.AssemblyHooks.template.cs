﻿// <auto-generated />
#pragma warning disable

using System.CodeDom.Compiler;
using System.Diagnostics;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using global::TechTalk.SpecFlow;
using global::TechTalk.SpecFlow.MSTest.SpecFlowPlugin;
using global::System.Runtime.CompilerServices;
using System.Threading.Tasks;

[GeneratedCode("SpecFlow", "SPECFLOW_VERSION")]
[TestClass]
public class PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks
{
    [AssemblyInitialize]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static async Task AssemblyInitializeAsync(TestContext testContext)
    {
        var currentAssembly = typeof(PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks).Assembly;
        var containerBuilder = new MsTestContainerBuilder(testContext);

        await global::TechTalk.SpecFlow.TestRunnerManager.OnTestRunStartAsync(currentAssembly, containerBuilder: containerBuilder);
    }

    [AssemblyCleanup]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static async Task AssemblyCleanupAsync()
    {
        var currentAssembly = typeof(PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks).Assembly;
        await global::TechTalk.SpecFlow.TestRunnerManager.OnTestRunEndAsync(currentAssembly);
    }
}
#pragma warning restore
