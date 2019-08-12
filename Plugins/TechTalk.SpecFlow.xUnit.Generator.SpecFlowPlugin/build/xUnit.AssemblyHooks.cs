using global::System.Reflection;
using global::System;
using global::Xunit;
using global::TechTalk.SpecFlow;
using global::TechTalk.SpecFlow.xUnit.SpecFlowPlugin;

[assembly:AssemblyFixture(typeof(XUnitAssemblyFixture))]

namespace InternalSpecFlow
{
    public class XUnitAssemblyFixture : IDisposable
    {
        private readonly Assembly _currentAssembly;

        public XUnitAssemblyFixture()
        {
            _currentAssembly = typeof(XUnitAssemblyFixture).Assembly;
            TestRunnerManager.OnTestRunStart(_currentAssembly);
        }

        public void Dispose()
        {
            TestRunnerManager.OnTestRunEnd(_currentAssembly);
        }
    }
}
