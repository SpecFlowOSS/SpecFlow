[assembly: global::TechTalk.SpecFlow.xUnit.SpecFlowPlugin.AssemblyFixture(typeof(global::InternalSpecFlow.XUnitAssemblyFixture))]

namespace InternalSpecFlow
{
    public class XUnitAssemblyFixture : global::System.IDisposable
    {
        private readonly global::System.Reflection.Assembly _currentAssembly;

        public XUnitAssemblyFixture()
        {
            _currentAssembly = typeof(XUnitAssemblyFixture).Assembly;
            global::TechTalk.SpecFlow.TestRunnerManager.OnTestRunStart(_currentAssembly);
        }

        public void Dispose()
        {
            global::TechTalk.SpecFlow.TestRunnerManager.OnTestRunEnd(_currentAssembly);
        }
    }
}
