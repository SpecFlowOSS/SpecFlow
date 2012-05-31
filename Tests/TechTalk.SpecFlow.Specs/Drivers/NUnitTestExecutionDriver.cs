using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class NUnitTestExecutionDriver
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly TestExecutionResult testExecutionResult;

        public NUnitTestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.testExecutionResult = testExecutionResult;
        }

        public string Include { get; set; }

        public TestRunSummary Execute()
        {
            string resultFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.xml");
            string logFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.txt");

            var args = new List<string>
                           {
                               inputProjectDriver.CompiledAssemblyPath,
                               "/xml:" + resultFilePath,
                               "/labels",
                               "/out:" + logFilePath
                           };
            if (Include != null)
                args.Add("/include:" + Include);

            NUnit.ConsoleRunner.Runner.Main(args.ToArray());

            return ProcessNUnitResult(logFilePath, resultFilePath);
        }

        public TestRunSummary ExecuteOutProc()
        {
            string resultFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.xml");
            string logFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.txt");

            var provessHelper = new ProcessHelper();
            var nunitConsolePath = Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), @"NUnit.Runners\tools\nunit-console-x86.exe");
            provessHelper.RunProcess(nunitConsolePath, "\"{0}\" \"/xml:{1}\" /labels \"/out={2}\" {3}", 
                inputProjectDriver.CompiledAssemblyPath, resultFilePath, logFilePath, GetIncludeExclude());

            return ProcessNUnitResult(logFilePath, resultFilePath);
        }

        private TestRunSummary ProcessNUnitResult(string logFilePath, string resultFilePath)
        {
            XDocument resultFileXml = XDocument.Load(resultFilePath);

            TestRunSummary summary = new TestRunSummary();

            summary.Total = resultFileXml.XPathSelectElements("//test-case").Count();
            summary.Succeeded = resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='True']").Count();
            summary.Failed =
                resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and failure]").Count();
            summary.Pending =
                resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and not(failure)]").Count
                    ();
            summary.Ignored = resultFileXml.XPathSelectElements("//test-case[@executed = 'False']").Count();

            testExecutionResult.LastExecutionSummary = summary;
            testExecutionResult.ExecutionLog = File.ReadAllText(logFilePath);

            Console.WriteLine("NUnit LOG:");
            Console.WriteLine(testExecutionResult.ExecutionLog);

            return summary;
        }

        private string GetIncludeExclude()
        {
            if (Include == null)
                return string.Empty;

            return string.Format(" \"/include:{0}\"", Include);
        }
    }
}
