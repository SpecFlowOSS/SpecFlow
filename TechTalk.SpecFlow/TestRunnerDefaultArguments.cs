using System.Threading.Tasks;

namespace TechTalk.SpecFlow
{
    public static class TestRunnerDefaultArguments
    {
        public static async Task GivenAsync(this ITestRunner testRunner, string text)
        {
            await testRunner.GivenAsync(text, null, null, null);
        }

        public static async Task WhenAsync(this ITestRunner testRunner, string text)
        {
            await testRunner.WhenAsync(text, null, null, null);
        }

        public static async Task ThenAsync(this ITestRunner testRunner, string text)
        {
            await testRunner.ThenAsync(text, null, null, null);
        }

        public static async Task AndAsync(this ITestRunner testRunner, string text)
        {
            await testRunner.AndAsync(text, null, null, null);
        }

        public static async Task ButAsync(this ITestRunner testRunner, string text)
        {
            await testRunner.ButAsync(text, null, null, null);
        }
    }
}
