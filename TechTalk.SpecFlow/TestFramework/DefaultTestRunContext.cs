using TechTalk.SpecFlow.EnvironmentAccess;

namespace TechTalk.SpecFlow.TestFramework
{
    public class DefaultTestRunContext : ITestRunContext
    {
        private readonly IEnvironmentWrapper _environmentWrapper;

        public DefaultTestRunContext(IEnvironmentWrapper environmentWrapper)
        {
            _environmentWrapper = environmentWrapper;
        }

        public string GetTestDirectory() => _environmentWrapper.GetCurrentDirectory();
    }
}
