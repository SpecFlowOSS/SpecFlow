using System.IO;
using TechTalk.SpecFlow.Tests.Bindings.Drivers;

namespace TechTalk.SpecFlow.Tests.Bindings.StepDefinitions
{
    [Binding]
    public sealed class BeforeScenarioHooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var directoryForTestProjects = InputProjectDriver.DetermineDirectoryForTestProjects();

            if (Directory.Exists(directoryForTestProjects))
            {
                Directory.Delete(directoryForTestProjects, true);
            }
        }
    }
}
