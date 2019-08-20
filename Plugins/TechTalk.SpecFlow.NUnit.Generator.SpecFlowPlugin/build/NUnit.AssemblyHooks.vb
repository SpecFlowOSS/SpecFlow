Imports NUnit.Framework
Imports TechTalk.SpecFlow
Imports System
Imports System.Reflection
Imports System.Threading.Tasks

<SetUpFixture>
Public NotInheritable Class NUnitAssemblyHooks
    <OneTimeSetUp>
    Public Async Function AssemblyInitializeAsync() As Task
        Dim currentAssembly = GetType(NUnitAssemblyHooks).Assembly
        Await TestRunnerManager.OnTestRunStartAsync(NameOf(NUnitAssemblyHooks), currentAssembly)
    End Function

    <OneTimeTearDown>
    Public Async Function AssemblyCleanupAsync() As Task
        Dim currentAssembly = GetType(NUnitAssemblyHooks).Assembly
        Await TestRunnerManager.OnTestRunEndAsync(currentAssembly)
    End Function

End Class
