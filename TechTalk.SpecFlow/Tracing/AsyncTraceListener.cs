using System;
using BoDi;

namespace TechTalk.SpecFlow.Tracing
{
    public class AsyncTraceListener : ITraceListener
    {
        private readonly Lazy<ITestRunner> _testRunner;
        private readonly ITraceListenerQueue _traceListenerQueue;

        public AsyncTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container)
        {
            _traceListenerQueue = traceListenerQueue;
            _testRunner = new Lazy<ITestRunner>(container.Resolve<ITestRunner>);
        }

        public virtual void WriteTestOutput(string message)
        {
            _traceListenerQueue.EnqueueMessage(_testRunner.Value, message, false);
        }

        public virtual void WriteToolOutput(string message)
        {
            _traceListenerQueue.EnqueueMessage(_testRunner.Value, message, true);
        }
    }
}