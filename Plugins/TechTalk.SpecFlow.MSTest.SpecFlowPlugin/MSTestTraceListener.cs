using System;
using BoDi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    class MSTestTraceListener : AsyncTraceListener
    {
        private readonly TestContext _testContext;
        private readonly Lazy<IContextManager> _contextManager;

        public MSTestTraceListener(ITraceListenerQueue traceListenerQueue, IObjectContainer container, TestContext testContext) : base(traceListenerQueue, container)
        {
            _testContext = testContext;
            _contextManager = new Lazy<IContextManager>(container.Resolve<IContextManager>);
        }

        private TestContext GetTestContext()
        {
            var scenarioContext = _contextManager.Value.ScenarioContext;
            
            // if we're not in the context of a scenario we use the global TestContext registered in the generated AssemblyInitialize class via the MsTestContainerBuilder
            if (scenarioContext == null)
                return _testContext;

            // if we're in the context of a scenario we use the test specific TestContext instance registered in the generated test class in the ScenarioInitialize method
            return scenarioContext.ScenarioContainer.Resolve<TestContext>();
        }

        public override void WriteTestOutput(string message)
        {
            GetTestContext().WriteLine(message);
            base.WriteTestOutput(message);
        }

        public override void WriteToolOutput(string message)
        {
            GetTestContext().WriteLine("-> " + message);
            base.WriteToolOutput(message);
        }
    }
}