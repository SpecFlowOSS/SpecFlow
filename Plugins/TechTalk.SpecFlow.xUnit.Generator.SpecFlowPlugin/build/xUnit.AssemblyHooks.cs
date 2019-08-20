using global::System;
using global::Xunit;
using global::TechTalk.SpecFlow;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace InternalSpecFlow
{
    public class XUnitAssemblyFixture
    {
        public static async Task InitializeAsync(string testClassId)
        {
            var currentAssembly = typeof(XUnitAssemblyFixture).Assembly;

            await TestRunnerManager.OnTestRunStartAsync(testClassId, currentAssembly);
        }
    }
}

