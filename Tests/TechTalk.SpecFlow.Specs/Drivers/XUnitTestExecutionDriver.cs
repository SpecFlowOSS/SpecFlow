using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class XUnitTestExecutionDriver
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly TestExecutionResult testExecutionResult;

        public XUnitTestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.testExecutionResult = testExecutionResult;
        }

        public TestRunSummary Execute()
        {
            string resultFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "xunit-result.xml");
            string logFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "xunit-result.txt");
            var xunitConsolePath = Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), @"xunit.runner.console\tools\xunit.console.exe");

            var provessHelper = new ProcessHelper();
            provessHelper.RunProcess(xunitConsolePath, "\"{0}\" -nunit \"{1}\"",
                inputProjectDriver.CompiledAssemblyPath, resultFilePath);
           
            File.WriteAllText(logFilePath, provessHelper.ConsoleOutput);
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

            Console.WriteLine("xUnit LOG:");
            Console.WriteLine(testExecutionResult.ExecutionLog);

            return summary;
        }
    }
}
