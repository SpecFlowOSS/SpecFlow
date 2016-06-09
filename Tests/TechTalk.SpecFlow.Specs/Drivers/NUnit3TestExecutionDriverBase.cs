using System;
using System.Collections.Generic;
using System.IO;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public abstract class NUnit3TestExecutionDriverBase : NUnitTestExecutionDriverBase
    {
        protected NUnit3TestExecutionDriverBase(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
            : base(inputProjectDriver, testExecutionResult)
        {
        }

        protected override sealed string GetIncludeExclude()
        {
            if (this.Include == null)
                return string.Empty;

            return string.Format(" \"--where:cat=={0}\"", this.Include);
        }

        public override sealed TestRunSummary Execute()
        {
            string resultFilePath = Path.Combine(this.InputProjectDriver.DeploymentFolder, "nunit-result.xml");
            string logFilePath = Path.Combine(this.InputProjectDriver.DeploymentFolder, "nunit-result.txt");

            var args = new List<string>
                           {
                               this.InputProjectDriver.CompiledAssemblyPath,
                               "--result=" + resultFilePath + ";format=nunit2",
                               "--labels=All",
                               "--output=" + logFilePath
                           };
            if (this.Include != null)
                args.Add(this.GetIncludeExclude());

            this.CallNUnit3(args);

            return this.ProcessNUnitResult(logFilePath, resultFilePath);
        }

        protected abstract void CallNUnit3(List<string> args);
    }
}