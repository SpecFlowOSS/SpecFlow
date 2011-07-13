using System.Reflection;

namespace TechTalk.SpecFlow
{
    public static class TestRunnerManager
    {
        public static ITestRunner GetTestRunner()
        {
            return ObjectContainer.EnsureSyncTestRunner(Assembly.GetCallingAssembly());
        }

        public static ITestRunner GetAsyncTestRunner()
        {
            return ObjectContainer.EnsureAsyncTestRunner(Assembly.GetCallingAssembly());
        }
    }
}