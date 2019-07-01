Namespace InternalSpecFlow

    Public Class XUnitAssemblyFixture

        Shared Sub New()
            Dim currentAssembly As System.Reflection.Assembly = GetType(XUnitAssemblyFixture).Assembly

            Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunStart(currentAssembly)
        End Sub

    End Class

End Namespace
