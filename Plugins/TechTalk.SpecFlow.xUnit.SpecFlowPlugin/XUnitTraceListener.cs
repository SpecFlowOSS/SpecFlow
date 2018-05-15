using System;
using BoDi;
using TechTalk.SpecFlow.Tracing;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    class XUnitTraceListener : AsyncTraceListener
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XUnitTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container, ITestOutputHelper testOutputHelper) : base(traceListenerQueue, container)
        {
            _testOutputHelper = testOutputHelper;
        }

        public override void WriteTestOutput(string message)
        {
            _testOutputHelper.WriteLine(message);
            base.WriteTestOutput(message);
        }

        public override void WriteToolOutput(string message)
        {
            _testOutputHelper.WriteLine("-> " + message);
            base.WriteToolOutput(message);
        }
    }
}