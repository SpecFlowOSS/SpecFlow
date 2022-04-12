﻿'<auto-generated />

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TechTalk.SpecFlow
Imports TechTalk.SpecFlow.MSTest.SpecFlowPlugin
Imports System
Imports System.Reflection
Imports System.CodeDom.Compiler
Imports System.Runtime.CompilerServices

<GeneratedCode("SpecFlow", "SPECFLOW_VERSION")>
<TestClass>
Public NotInheritable Class PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks
    <AssemblyInitialize>
    <MethodImpl(MethodImplOptions.NoInlining)>
    Public Shared Async Function AssemblyInitializeAsync(testContext As TestContext) As Task
        Dim currentAssembly As Assembly = GetType(PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks).Assembly
        Dim containerBuilder As New MsTestContainerBuilder(testContext)
        'TODO: Review/handle parallel execution with async
        Await Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunStartAsync("TBD", currentAssembly, containerBuilder)
    End Function

    <AssemblyCleanup>
    <MethodImpl(MethodImplOptions.NoInlining)>
    Public Shared Async Function AssemblyCleanupAsync() As Task
        Dim currentAssembly As Assembly = GetType(PROJECT_ROOT_NAMESPACE_MSTestAssemblyHooks).Assembly
        Await Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunEndAsync(currentAssembly)
    End Function

End Class
