using System;
using NUnit.Framework;

namespace TechTalk.SpecFlow.Tracing
{
    public class DefaultListener : ITraceListener
    {
        public void SetTestname(string name)
        {
            throw new NotImplementedException();
        }

        public void WriteTestOutput(string message)
        {
            TestContext.Out.WriteLine(message);
        }

        public void WriteToolOutput(string message)
        {
            TestContext.Out.WriteLine($"-> {message}");
        }
    }
}