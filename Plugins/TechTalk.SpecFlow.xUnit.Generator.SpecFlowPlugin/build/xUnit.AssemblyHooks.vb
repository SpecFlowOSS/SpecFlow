Imports System.Threading.Tasks
Imports TechTalk.SpecFlow

Namespace InternalSpecFlow

    Public Class XUnitAssemblyFixture

        Public Shared Async Function InitializeAsync(ByVal testClassId As String) As Task
            Dim currentAssembly = GetType(XUnitAssemblyFixture).Assembly
            Await TestRunnerManager.OnTestRunStartAsync(testClassId, currentAssembly)
        End Function

    End Class

End Namespace
