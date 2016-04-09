using System;
using BoDi;
using NUnit.Framework;

namespace TechTalk.SpecFlow.Tracing
{
    public class NUnit3TraceListener : ITraceListener
    {
        private readonly Lazy<ITestRunner> testRunner;

        public NUnit3TraceListener(IObjectContainer container)
        {
            testRunner = new Lazy<ITestRunner>(container.Resolve<ITestRunner>);
        }

        public void WriteTestOutput(string message)
        {
            message = FormatMessage(message);
            TestContext.Out.WriteLine(message);
        }

        public void WriteToolOutput(string message)
        {
            message = FormatMessage(message);
            TestContext.Out.WriteLine($"-> {message}");
        }

        private string FormatMessage(string message)
        {
            var testName = GetCurrentTestCaseName();
            return $"#{testRunner.Value.ThreadId}: [{testName}]: {message}";
        }

        private string GetCurrentTestCaseName()
        {
            return TestContext.CurrentContext.Test.FullName;
        }
    }
}
