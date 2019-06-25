Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TechTalk.SpecFlow

Namespace TechTalk.SpecFlow.MSTest.Generator.SpecFlowPlugin.build

    Public NotInheritable Class MSTestAssemblyHooks
        <AssemblyInitialize>
        Public Shared Sub AssemblyInitialize()
            TestRunnerManager.GetTestRunner().OnTestRunStart()
        End Sub

        <AssemblyCleanup>
        Public Shared Sub AssemblyCleanup()
            TestRunnerManager.OnTestRunEnd()
        End Sub

    End Class
End Namespace
