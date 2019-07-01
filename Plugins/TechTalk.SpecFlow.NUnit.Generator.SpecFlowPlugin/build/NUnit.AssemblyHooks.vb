Imports NUnit.Framework
Imports TechTalk.SpecFlow
Imports System
Imports System.Reflection

<SetUpFixture>
Public NotInheritable Class NUnitAssemblyHooks
    <OneTimeSetUp>
    Public Shared Sub AssemblyInitialize()
        Dim currentAssembly As Assembly = GetType(NUnitAssemblyHooks).Assembly

        TestRunnerManager.OnTestRunStart(currentAssembly)
    End Sub

    <OneTimeTearDown>
    Public Shared Sub AssemblyCleanup()
        Dim currentAssembly As Assembly = GetType(NUnitAssemblyHooks).Assembly

        TestRunnerManager.OnTestRunEnd(currentAssembly)
    End Sub

End Class
