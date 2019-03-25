using System;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    class XUnitTraceListener : AsyncTraceListener
    {
        private readonly Lazy<IContextManager> _contextManager;

        public XUnitTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container) : base(traceListenerQueue, container)
        {
            _contextManager = new Lazy<IContextManager>(container.Resolve<IContextManager>);
        }

        private ITestOutputHelper GetTestOutputHelper()
        {
            var scenarioContext = _contextManager.Value.ScenarioContext;
            if (scenarioContext == null)
                return null;

            if (!scenarioContext.ScenarioContainer.IsRegistered<ITestOutputHelper>())
                return null;

            return scenarioContext.ScenarioContainer.Resolve<ITestOutputHelper>();
        }

        public override void WriteTestOutput(string message)
        {
            var testOutputHelper = GetTestOutputHelper();
            if (testOutputHelper != null)
                testOutputHelper.WriteLine(message);
            else
                base.WriteTestOutput(message);
        }

        public override void WriteToolOutput(string message)
        {
            var testOutputHelper = GetTestOutputHelper();
            if (testOutputHelper != null)
                testOutputHelper.WriteLine("-> " + message);
            else
                base.WriteToolOutput(message);
        }
    }
}