using System.Diagnostics;
using global::System;
using global::Xunit;
using global::TechTalk.SpecFlow;


[assembly: TestFramework("TechTalk.SpecFlow.xUnit.SpecFlowPlugin.Extensions.AssemblyFixture.XunitTestFrameworkWithAssemblyFixture", "TechTalk.SpecFlow.xUnit.SpecFlowPlugin")]

[assembly: TechTalk.SpecFlow.xUnit.SpecFlowPlugin.Extensions.AssemblyFixture.AssemblyFixture(typeof(TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin.MyAssemblyFixture))]

namespace TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin
{

    public class MyAssemblyFixture : IDisposable
    {
        

        public MyAssemblyFixture()
        {
            Debugger.Launch();

            TestRunnerManager.GetTestRunner().OnTestRunStart();
        }

        public void Dispose()
        {
            TestRunnerManager.OnTestRunEnd();
        }
    }
}
