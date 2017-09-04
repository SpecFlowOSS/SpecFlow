using System;
using System.IO;

using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
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
