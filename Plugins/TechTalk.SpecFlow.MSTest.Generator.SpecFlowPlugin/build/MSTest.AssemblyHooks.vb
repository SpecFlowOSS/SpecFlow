Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TechTalk.SpecFlow
Imports System
Imports System.Reflection
Imports System.Threading.Tasks


<TestClass>
    Public NotInheritable Class MSTestAssemblyHooks
    <AssemblyInitialize>
    Public Shared Async Function AssemblyInitializeAsync(ByVal testContext As TestContext) As Task
        Dim currentAssembly = GetType(MSTestAssemblyHooks).Assembly
        Await TestRunnerManager.OnTestRunStartAsync(NameOf(MSTestAssemblyHooks), currentAssembly)
    End Function

    <AssemblyCleanup>
    Public Shared Async Function AssemblyCleanupAsync() As Task
        Dim currentAssembly = GetType(MSTestAssemblyHooks).Assembly
        Await TestRunnerManager.OnTestRunEndAsync(currentAssembly)
    End Function
End Class
