using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public sealed class AfterScenarioHooks
    {
        [AfterTestRun]
        public static void AfterTestRun()
        {
            var directoryForTestProjects = InputProjectDriver.DetermineDirectoryForTestProjects();

            if (Directory.Exists(directoryForTestProjects))
            {
                Directory.Delete(directoryForTestProjects, true);
            }
        }
    }
}
