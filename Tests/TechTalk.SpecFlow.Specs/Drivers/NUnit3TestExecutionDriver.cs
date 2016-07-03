using System;
using System.Collections.Generic;
using System.IO;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class NUnit3TestExecutionDriver : NUnit3TestExecutionDriverBase
    {
        public NUnit3TestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
            : base(inputProjectDriver, testExecutionResult)
        {
        }

        protected override void CallNUnit3(List<string> arguments)
        {
            string nunitConsoleRunnerPath = @"NUnit3-Runner\bin\nunit-console.exe";
            var nunitConsolePath = Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), nunitConsoleRunnerPath);
            var processHelper = new ProcessHelper();
            processHelper.RunProcess(nunitConsolePath, string.Join(" ", arguments));
        }
    }
}
