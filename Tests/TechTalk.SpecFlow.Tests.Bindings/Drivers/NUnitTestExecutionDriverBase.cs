using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public abstract class NUnitTestExecutionDriverBase
    {
        protected InputProjectDriver InputProjectDriver { get; }

        protected readonly TestExecutionResult testExecutionResult;

        protected NUnitTestExecutionDriverBase(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
        {
            this.InputProjectDriver = inputProjectDriver;
            this.testExecutionResult = testExecutionResult;
        }

        public string Include { get; set; }

        public abstract TestRunSummary Execute();

        protected virtual TestRunSummary ProcessNUnitResult(string logFilePath, string resultFilePath)
        {
            XDocument resultFileXml = XDocument.Load(resultFilePath);

            TestRunSummary summary = new TestRunSummary();

            summary.Total = resultFileXml.XPathSelectElements("//test-case").Count();
            summary.Succeeded = resultFileXml.XPathSelectElements("//test-case[@runstate = 'Runnable' and @result='Passed']").Count();
            summary.Failed = resultFileXml.XPathSelectElements("//test-case[@runstate = 'Runnable' and @result='Failed']").Count();
            summary.Pending = resultFileXml.XPathSelectElements("//test-case[@runstate = 'Runnable' and @result='Inconclusive']").Count();
            summary.Ignored = resultFileXml.XPathSelectElements("//test-case[@runstate = 'Ignored']").Count();

            this.testExecutionResult.LastExecutionSummary = summary;
            this.testExecutionResult.ExecutionLog = File.ReadAllText(logFilePath);

            Console.WriteLine("NUnit LOG:");
            Console.WriteLine(this.testExecutionResult.ExecutionLog);

            return summary;
        }

        protected abstract string GetIncludeExclude();
    }
}