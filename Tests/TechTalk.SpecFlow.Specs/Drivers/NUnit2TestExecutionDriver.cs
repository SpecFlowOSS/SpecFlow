using System;
using System.IO;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class NUnit2TestExecutionDriver : NUnitTestExecutionDriverBase
    {
        public NUnit2TestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
            : base(inputProjectDriver, testExecutionResult)
        {
        }

        public override TestRunSummary Execute()
        {
            string resultFilePath = Path.Combine(this.InputProjectDriver.DeploymentFolder, "nunit-result.xml");
            string logFilePath = Path.Combine(this.InputProjectDriver.DeploymentFolder, "nunit-result.txt");

            var provessHelper = new ProcessHelper();
            var nunitConsolePath = Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), @"NUnit.Runners\tools\nunit-console-x86.exe");
            provessHelper.RunProcess(nunitConsolePath, "\"{0}\" \"/xml:{1}\" /labels \"/out={2}\" {3}",
                this.InputProjectDriver.CompiledAssemblyPath, resultFilePath, logFilePath, this.GetIncludeExclude());

            return this.ProcessNUnitResult(logFilePath, resultFilePath);
        }

        protected override string GetIncludeExclude()
        {
            if (this.Include == null)
                return string.Empty;

            return string.Format(" \"/include:{0}\"", this.Include);
        }
    }
}