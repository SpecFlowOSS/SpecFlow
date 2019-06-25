Imports System
Imports Xunit
Imports TechTalk.SpecFlow

Namespace TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin.build

    Public Class XUnitAssemblyHooksFixture
        Implements IDisposable

        Public Sub New()
            TestRunnerManager.GetTestRunner().OnTestRunStart()
        End Sub

        Public Sub Dispose()
            TestRunnerManager.OnTestRunEnd()
        End Sub

    End Class

    Public Class XUnitAssemblyHooksCollectionDefinition
        Implements ICollectionFixture(Of XUnitAssemblyHooksFixture)
    End Class
End Namespace
