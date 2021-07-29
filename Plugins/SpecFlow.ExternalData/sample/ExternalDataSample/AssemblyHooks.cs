using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Specs
{
    // This class is only required because in this sample application the generator 
    // is not loaded from NuGet package. In a usual SpecFlow project it is not needed.
    [SetUpFixture]
    public class NUnitAssemblyHooks
    {
        [OneTimeSetUp]
        public void AssemblyInitialize()
        {
            var currentAssembly = typeof(NUnitAssemblyHooks).Assembly;
            TestRunnerManager.OnTestRunStart(currentAssembly);
        }

        [OneTimeTearDown]
        public void AssemblyCleanup()
        {
            var currentAssembly = typeof(NUnitAssemblyHooks).Assembly;
            TestRunnerManager.OnTestRunEnd(currentAssembly);
        }
    }
}
