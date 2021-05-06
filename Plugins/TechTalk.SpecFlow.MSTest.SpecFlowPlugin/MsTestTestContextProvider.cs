using System;
using BoDi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    public interface IMSTestTestContextProvider
    {
        TestContext GetTestContext();
    }
    
    public class MSTestTestContextProvider : IMSTestTestContextProvider
    {
        private readonly TestContext _testContext;
        private readonly Lazy<IContextManager> _contextManager;

        public MSTestTestContextProvider(IObjectContainer container, TestContext testContext)
        {
            _testContext = testContext;
            _contextManager = new Lazy<IContextManager>(container.Resolve<IContextManager>);
        }
        
        public TestContext GetTestContext()
        {
            var scenarioContext = _contextManager.Value.ScenarioContext;
            
            // if we're not in the context of a scenario we use the global TestContext registered in the generated AssemblyInitialize class via the MsTestContainerBuilder
            if (scenarioContext == null)
                return _testContext;

            // if we're in the context of a scenario we use the test specific TestContext instance registered in the generated test class in the ScenarioInitialize method
            return scenarioContext.ScenarioContainer.Resolve<TestContext>();
        }
    }
}
