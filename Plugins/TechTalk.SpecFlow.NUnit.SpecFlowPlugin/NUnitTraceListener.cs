using BoDi;
using NUnit.Framework;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class NUnitTraceListener : AsyncTraceListener
    {
        public NUnitTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container) : base(traceListenerQueue, container)
        {
        }

        public override void WriteTestOutput(string message)
        {
            TestContext.WriteLine(message);
            base.WriteTestOutput(message);
        }

        public override void WriteToolOutput(string message)
        {
            TestContext.WriteLine("-> " + message);
            base.WriteToolOutput(message);
        }
    }
}
