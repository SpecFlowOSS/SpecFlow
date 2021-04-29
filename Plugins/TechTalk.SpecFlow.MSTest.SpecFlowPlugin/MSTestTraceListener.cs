using BoDi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    class MSTestTraceListener : AsyncTraceListener
    {
        private readonly TestContext _testContext;

        public MSTestTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container, TestContext testContext) : base(traceListenerQueue, container)
        {
            _testContext = testContext;
        }


        public override void WriteTestOutput(string message)
        {
            _testContext.WriteLine(message);
            base.WriteTestOutput(message);
        }

        public override void WriteToolOutput(string message)
        {
            _testContext.WriteLine("-> " + message);
            base.WriteToolOutput(message);
        }
    }
}