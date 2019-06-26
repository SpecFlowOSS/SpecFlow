using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using global::TechTalk.SpecFlow;

namespace TechTalk.SpecFlow.MSTest.Generator.SpecFlowPlugin.build
{
    public class MSTestAssemblyHooks
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize()
        {
            TestRunnerManager.GetTestRunner().OnTestRunStart();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            TestRunnerManager.OnTestRunEnd();
        }
    }
}
