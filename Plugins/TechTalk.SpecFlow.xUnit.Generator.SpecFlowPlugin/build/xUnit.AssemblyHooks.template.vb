﻿'<auto-generated />

Imports System.CodeDom.Compiler
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports System.Reflection

<Assembly: Global.Xunit.TestFramework("TechTalk.SpecFlow.xUnit.SpecFlowPlugin.XunitTestFrameworkWithAssemblyFixture", "TechTalk.SpecFlow.xUnit.SpecFlowPlugin")>
<Assembly: Global.TechTalk.SpecFlow.xUnit.SpecFlowPlugin.AssemblyFixture(GetType(PROJECT_ROOT_NAMESPACE_XUnitAssemblyFixture))>

<GeneratedCode("SpecFlow", "SPECFLOW_VERSION")>
Public Class PROJECT_ROOT_NAMESPACE_XUnitAssemblyFixture
    Implements Global.System.IAsyncDisposable

    <MethodImpl(MethodImplOptions.NoInlining)>
    Public Async Function InitializeAsync(ByVal testClassId As String) As Task
        Dim currentAssembly As Assembly = GetType(PROJECT_ROOT_NAMESPACE_XUnitAssemblyFixture).Assembly
        Await Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunStartAsync(testClassId, currentAssembly)
    End Function

    <MethodImpl(MethodImplOptions.NoInlining)>
    Public Function DisposeAsync() As ValueTask Implements Global.System.IAsyncDisposable.DisposeAsync
        Return New ValueTask(DisposeAsyncAsTask())
    End Function

    <MethodImpl(MethodImplOptions.NoInlining)>
    Private Async Function DisposeAsyncAsTask() As Task
        Dim currentAssembly As Assembly = GetType(PROJECT_ROOT_NAMESPACE_XUnitAssemblyFixture).Assembly
        Await Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunEndAsync(currentAssembly)
    End Function
End Class

<Global.Xunit.CollectionDefinition("SpecFlowNonParallelizableFeatures", DisableParallelization:=True)>
Public Class PROJECT_ROOT_NAMESPACE_SpecFlowNonParallelizableFeaturesCollectionDefinition
End Class