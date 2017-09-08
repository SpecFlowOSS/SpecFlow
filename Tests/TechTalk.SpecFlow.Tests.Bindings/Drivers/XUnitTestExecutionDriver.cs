using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class XUnitTestExecutionDriver
    {
        private readonly InputProjectDriver _inputProjectDriver;
        private readonly TestExecutionResult _testExecutionResult;
        private readonly Folders _folders;

        public XUnitTestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult, Folders folders)
        {
            _inputProjectDriver = inputProjectDriver;
            _testExecutionResult = testExecutionResult;
            _folders = folders;
        }

        public TestRunSummary Execute()
        {
            string resultFilePath = Path.Combine(_inputProjectDriver.DeploymentFolder, "xunit-result.xml");
            string logFilePath = Path.Combine(_inputProjectDriver.DeploymentFolder, "xunit-result.txt");
            var xunitConsolePath = Path.Combine(_folders.PackageFolder, "xunit.runner.console","2.2.0","tools","xunit.console.exe");

            var provessHelper = new ProcessHelper();
            provessHelper.RunProcess(xunitConsolePath, "\"{0}\" -xml \"{1}\"", _inputProjectDriver.CompiledAssemblyPath, resultFilePath);

            File.WriteAllText(logFilePath, provessHelper.ConsoleOutput);
            return ProcessXUnitResult(logFilePath, resultFilePath);
        }

        private TestRunSummary ProcessXUnitResult(string logFilePath, string resultFilePath)
        {
            XDocument resultFileXml = XDocument.Load(resultFilePath);

            TestRunSummary summary = new TestRunSummary();

            summary.Total = resultFileXml.XPathSelectElements("//test").Count();
            summary.Succeeded = resultFileXml.XPathSelectElements("//test[@result = 'Pass']").Count();
            summary.Failed = resultFileXml.XPathSelectElements("//test[@result = 'Fail']").Count();
            summary.Ignored = resultFileXml.XPathSelectElements("//test[@result = 'Skip']").Count();

            _testExecutionResult.LastExecutionSummary = summary;
            _testExecutionResult.ExecutionLog = File.ReadAllText(logFilePath);

            Console.WriteLine("xUnit LOG:");
            Console.WriteLine(_testExecutionResult.ExecutionLog);

            return summary;
        }        
    }
}
