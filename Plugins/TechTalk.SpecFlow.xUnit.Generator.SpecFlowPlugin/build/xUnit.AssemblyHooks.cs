using global::System;
using global::Xunit;
using global::TechTalk.SpecFlow;

namespace TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin.build
{
    public class XUnitAssemblyHooksFixture : IDisposable
    {
        public XUnitAssemblyHooksFixture()
        {
            TestRunnerManager.GetTestRunner().OnTestRunStart();
        }

        public void Dispose()
        {
            TestRunnerManager.OnTestRunEnd();
        }
    }

    [CollectionDefinition("SpecFlowXUnitHooks")]
    public class XUnitAssemblyHooksCollectionDefinition : ICollectionFixture<XUnitAssemblyHooksFixture>
    {
    }
}
