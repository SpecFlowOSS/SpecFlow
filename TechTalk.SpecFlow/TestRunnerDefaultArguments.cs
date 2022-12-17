using System.Threading.Tasks;

namespace TechTalk.SpecFlow
{
    public static class TestRunnerDefaultArguments
    {
        public static Task GivenAsync(this ITestRunner testRunner, string text)
        {
            return testRunner.GivenAsync(text, null, null, null);
        }

        public static Task WhenAsync(this ITestRunner testRunner, string text)
        {
            return testRunner.WhenAsync(text, null, null, null);
        }

        public static Task ThenAsync(this ITestRunner testRunner, string text)
        {
            return testRunner.ThenAsync(text, null, null, null);
        }

        public static Task AndAsync(this ITestRunner testRunner, string text)
        {
            return testRunner.AndAsync(text, null, null, null);
        }

        public static Task ButAsync(this ITestRunner testRunner, string text)
        {
            return testRunner.ButAsync(text, null, null, null);
        }
    }
}
