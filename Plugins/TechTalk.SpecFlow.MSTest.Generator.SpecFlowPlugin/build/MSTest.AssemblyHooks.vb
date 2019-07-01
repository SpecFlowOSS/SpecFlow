Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TechTalk.SpecFlow
Imports System
Imports System.Reflection


<TestClass>
    Public NotInheritable Class MSTestAssemblyHooks
        <AssemblyInitialize>
        Public Shared Sub AssemblyInitialize(testContext As TestContext)

            Dim currentAssembly As Assembly = GetType(MSTestAssemblyHooks).Assembly

            TestRunnerManager.OnTestRunStart(currentAssembly)
        End Sub

        <AssemblyCleanup>
        Public Shared Sub AssemblyCleanup()

            Dim currentAssembly As Assembly = GetType(MSTestAssemblyHooks).Assembly

            TestRunnerManager.OnTestRunEnd(currentAssembly)
        End Sub

    End Class
