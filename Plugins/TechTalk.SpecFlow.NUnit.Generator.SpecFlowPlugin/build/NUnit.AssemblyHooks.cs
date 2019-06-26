using global::NUnit.Framework;
using global::TechTalk.SpecFlow;

namespace TechTalk.SpecFlow.NUnit.Generator.SpecFlowPlugin.build
{
    public class NUnitAssemblyHooks
    {
        [OneTimeSetUp]
        public static void AssemblyInitialize()
        {
            TestRunnerManager.GetTestRunner().OnTestRunStart();
        }

        [OneTimeTearDown]
        public static void AssemblyCleanup()
        {
            TestRunnerManager.OnTestRunEnd();
        }
    }
}
