using System;
using System.Linq;

namespace TechTalk.SpecFlow.Tracing
{
    public class AsyncTraceListener : ITraceListener
    {
        private readonly ITraceListenerQueue traceListenerQueue;
        private readonly ITestRunner testRunner;

        public AsyncTraceListener(ITraceListenerQueue traceListenerQueue)
        {
            this.traceListenerQueue = traceListenerQueue;
            this.testRunner = null;
        }

        public void WriteTestOutput(string message)
        {
            traceListenerQueue.EnqueueMessgage(testRunner, message, false);
        }

        public void WriteToolOutput(string message)
        {
            traceListenerQueue.EnqueueMessgage(testRunner, message, true);
        }
    }
}
