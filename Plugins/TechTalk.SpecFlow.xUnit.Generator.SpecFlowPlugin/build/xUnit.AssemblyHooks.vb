<Assembly: Global.TechTalk.SpecFlow.xUnit.SpecFlowPlugin.AssemblyFixture(GetType(InternalSpecFlow.XUnitAssemblyFixture))>

Namespace InternalSpecFlow

    Public Class XUnitAssemblyFixture
        Implements Global.System.IDisposable

        Private ReadOnly _currentAssembly As Global.System.Reflection.Assembly

        Public Sub New()
            _currentAssembly = GetType(XUnitAssemblyFixture).Assembly
            Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunStart(_currentAssembly)
        End Sub


        Public Sub Dispose() Implements Global.System.IDisposable.Dispose
            Global.TechTalk.SpecFlow.TestRunnerManager.OnTestRunEnd(_currentAssembly)
        End Sub
    End Class

End Namespace
