using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Infrastructure.ContextManagers;

namespace TechTalk.SpecFlow.UnitTestProvider
{
	public class XUnitRuntimeProvider : IUnitTestRuntimeProvider
	{
        public void TestPending(string message)
        {
            throw new SpecFlowException("Test pending: " + message);
        }

        public void TestInconclusive(string message)
		{
		    throw new SpecFlowException("Test inconclusive: " + message);
		}

		public void TestIgnore(string message)
		{
            throw new SpecFlowException("Test ignored: " + message);
		}

		public bool DelayedFixtureTearDown
		{
			get { return false; }
		}

	    public void RegisterContextManagers(IObjectContainer objectContainer)
	    {
            objectContainer.RegisterTypeAs<InternalContextManager<FeatureContext>, IInternalContextManager<FeatureContext>>();
            objectContainer.RegisterTypeAs<InternalContextManager<ScenarioContext>, IInternalContextManager<ScenarioContext>>();
            objectContainer.RegisterTypeAs<StackedInternalContextManager<ScenarioStepContext>, IInternalContextManager<ScenarioStepContext>>();
        }
	}
}