using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
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

        protected override TestRunSummary ProcessNUnitResult(string logFilePath, string resultFilePath)
        {
            XDocument resultFileXml = XDocument.Load(resultFilePath);

            TestRunSummary summary = new TestRunSummary();

            summary.Total = resultFileXml.XPathSelectElements("//test-case").Count();
            summary.Succeeded = resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='True']").Count();
            summary.Failed = resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and failure]").Count();
            summary.Pending =  resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and not(failure)]").Count();
            summary.Ignored = resultFileXml.XPathSelectElements("//test-case[@executed = 'False']").Count();

            this.testExecutionResult.LastExecutionSummary = summary;
            this.testExecutionResult.ExecutionLog = File.ReadAllText(logFilePath);

            Console.WriteLine("NUnit LOG:");
            Console.WriteLine(this.testExecutionResult.ExecutionLog);

            return summary;
        }

        protected override string GetIncludeExclude()
        {
            if (this.Include == null)
                return string.Empty;

            return string.Format(" \"/include:{0}\"", this.Include);
        }
    }
}