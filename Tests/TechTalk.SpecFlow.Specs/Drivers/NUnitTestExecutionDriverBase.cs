using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public abstract class NUnitTestExecutionDriverBase
    {
        protected InputProjectDriver InputProjectDriver { get; }

        private readonly TestExecutionResult testExecutionResult;

        protected NUnitTestExecutionDriverBase(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
        {
            this.InputProjectDriver = inputProjectDriver;
            this.testExecutionResult = testExecutionResult;
        }

        public string Include { get; set; }

        public abstract TestRunSummary Execute();

        protected TestRunSummary ProcessNUnitResult(string logFilePath, string resultFilePath)
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

            this.testExecutionResult.LastExecutionSummary = summary;
            this.testExecutionResult.ExecutionLog = File.ReadAllText(logFilePath);

            Console.WriteLine("NUnit LOG:");
            Console.WriteLine(this.testExecutionResult.ExecutionLog);

            return summary;
        }

        protected abstract string GetIncludeExclude();
    }
}