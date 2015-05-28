using System;
using System.Linq;
using BoDi;

namespace TechTalk.SpecFlow.Tracing
{
    public class AsyncTraceListener : ITraceListener
    {
        private readonly ITraceListenerQueue traceListenerQueue;
        private readonly Lazy<ITestRunner> testRunner;

        public AsyncTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container)
        {
            this.traceListenerQueue = traceListenerQueue;
            this.testRunner = new Lazy<ITestRunner>(container.Resolve<ITestRunner>);
        }

        public void WriteTestOutput(string message)
        {
            traceListenerQueue.EnqueueMessgage(testRunner.Value, message, false);
        }

        public void WriteToolOutput(string message)
        {
            traceListenerQueue.EnqueueMessgage(testRunner.Value, message, true);
        }
    }
}
