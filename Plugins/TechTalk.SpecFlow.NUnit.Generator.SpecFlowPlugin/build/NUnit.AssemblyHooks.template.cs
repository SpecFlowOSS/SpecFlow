﻿// <auto-generated />
#pragma warning disable

using System.CodeDom.Compiler;
using System.Diagnostics;
using global::System.Runtime.CompilerServices;
using System.Threading.Tasks;

[GeneratedCode("SpecFlow", "SPECFLOW_VERSION")]
[global::NUnit.Framework.SetUpFixture]
public class PROJECT_ROOT_NAMESPACE_NUnitAssemblyHooks
{
    [global::NUnit.Framework.OneTimeSetUp]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public async Task AssemblyInitializeAsync()
    {
        var currentAssembly = typeof(PROJECT_ROOT_NAMESPACE_NUnitAssemblyHooks).Assembly;
        await global::TechTalk.SpecFlow.TestRunnerManager.OnTestRunStartAsync(currentAssembly);
    }

    [global::NUnit.Framework.OneTimeTearDown]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public async ValueTask AssemblyCleanupAsync()
    {
        var currentAssembly = typeof(PROJECT_ROOT_NAMESPACE_NUnitAssemblyHooks).Assembly;
        await global::TechTalk.SpecFlow.TestRunnerManager.OnTestRunEndAsync(currentAssembly);
    }
}
#pragma warning restore
