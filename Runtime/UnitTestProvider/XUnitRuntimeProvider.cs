using System;
using System.Linq;

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
	}
}