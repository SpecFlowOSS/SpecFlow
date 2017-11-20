using System.Collections.Generic;
using System.IO;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class NUnit3TestExecutionDriver : NUnit3TestExecutionDriverBase
    {
        public NUnit3TestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
            : base(inputProjectDriver, testExecutionResult)
        {
        }

        protected override void CallNUnit3(List<string> arguments)
        {
            string nunitConsoleRunnerPath = @"NUnit3-Runner\tools\nunit3-console.exe";
            var nunitConsolePath = Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), nunitConsoleRunnerPath);
            var processHelper = new ProcessHelper();
            processHelper.RunProcess(nunitConsolePath, string.Join(" ", arguments));
        }
    }
}
