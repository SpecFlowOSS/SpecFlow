using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
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

        private string GetAssemblyFolder()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(assemblyFolder != null);
            return assemblyFolder;
        }

        public TestRunSummary Execute()
        {
            var nunitConsolePath = Path.Combine(GetAssemblyFolder(), @"NUnit\tools\nunit-console-x86.exe");

            string logFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.xml");

            ProcessStartInfo psi = new ProcessStartInfo(nunitConsolePath, string.Format("\"{0}\" \"/xml:{1}\"", 
                inputProjectDriver.CompiledAssemblyPath, logFilePath));
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            var p = Process.Start(psi);

            Console.WriteLine(p.StandardOutput.ReadToEnd());

            p.WaitForExit();

            XDocument logFile = XDocument.Load(logFilePath);

            TestRunSummary summary = new TestRunSummary();

//            XmlNameTable nameTable = new NameTable();
//            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
//            namespaceManager.AddNamespace("nunit", "");
//
            summary.Total = logFile.XPathSelectElements("//test-case").Count();
            summary.Succeeded = logFile.XPathSelectElements("//test-case[@executed = 'True' and @success='True']").Count();
            summary.Failed = logFile.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and failure]").Count();
            summary.Pending = logFile.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and not(failure)]").Count();
            summary.Ignored = logFile.XPathSelectElements("//test-case[@executed = 'False']").Count();

            testExecutionResult.LastExecutionSummary = summary;

            return summary;
        }
    }
}
