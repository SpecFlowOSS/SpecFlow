﻿'<auto-generated />

Imports NUnit.Framework
Imports TechTalk.SpecFlow
Imports System
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.Runtime.CompilerServices

<GeneratedCode("SpecFlow", "SPECFLOW_VERSION")>
<SetUpFixture>
Public NotInheritable Class PROJECT_ROOT_NAMESPACE_NUnitAssemblyHooks
    <OneTimeSetUp>
    <MethodImpl(MethodImplOptions.NoInlining)>
    Public Async Function AssemblyInitializeAsync() As Task
        Dim currentAssembly As Assembly = GetType(PROJECT_ROOT_NAMESPACE_NUnitAssemblyHooks).Assembly
        Await Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunStartAsync(currentAssembly)
    End Function

    <OneTimeTearDown>
    <MethodImpl(MethodImplOptions.NoInlining)>
    Public Async Function AssemblyCleanupAsync() As Task
        Dim currentAssembly As Assembly = GetType(PROJECT_ROOT_NAMESPACE_NUnitAssemblyHooks).Assembly
        Await Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunEndAsync(currentAssembly)
    End Function

End Class
