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

            string resultFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.xml");
            string logFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.txt");

            var provessHelper = new ProcessHelper();

            provessHelper.RunProcess(nunitConsolePath, "\"{0}\" \"/xml:{1}\" /labels \"/out={2}\" ", 
                inputProjectDriver.CompiledAssemblyPath, resultFilePath, logFilePath);

            XDocument resultFileXml = XDocument.Load(resultFilePath);

            TestRunSummary summary = new TestRunSummary();

//            XmlNameTable nameTable = new NameTable();
//            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
//            namespaceManager.AddNamespace("nunit", "");
//
            summary.Total = resultFileXml.XPathSelectElements("//test-case").Count();
            summary.Succeeded = resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='True']").Count();
            summary.Failed = resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and failure]").Count();
            summary.Pending = resultFileXml.XPathSelectElements("//test-case[@executed = 'True' and @success='False' and not(failure)]").Count();
            summary.Ignored = resultFileXml.XPathSelectElements("//test-case[@executed = 'False']").Count();

            testExecutionResult.LastExecutionSummary = summary;

            return summary;
        }
    }
}
