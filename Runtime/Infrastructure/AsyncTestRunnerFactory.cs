using BoDi;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    internal class AsyncTestRunnerFactory : TestRunnerFactory
    {
        public AsyncTestRunnerFactory(IObjectContainer objectContainer, RuntimeConfiguration runtimeConfiguration) : base(objectContainer, runtimeConfiguration)
        {
        }

        protected override ITestRunner CreateTestRunnerInstance()
        {
            return new AsyncTestRunner(base.CreateTestRunnerInstance());
        }
    }
}