Imports NUnit.Framework
Imports TechTalk.SpecFlow

Namespace TechTalk.SpecFlow.NUnit.Generator.SpecFlowPlugin.build

    Public NotInheritable Class NUnitAssemblyHooks
        <OneTimeSetUp>
        Public Shared Sub AssemblyInitialize()
            TestRunnerManager.GetTestRunner().OnTestRunStart()
        End Sub

        <OneTimeTearDown>
        Public Shared Sub AssemblyCleanup()
            TestRunnerManager.OnTestRunEnd()
        End Sub

    End Class
End Namespace
