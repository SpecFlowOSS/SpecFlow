using NUnit.Framework;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class NUnitRuntimeProvider : IUnitTestRuntimeProvider
    {
        public void TestPending(string message)
        {
            TestInconclusive(message);
        }

        public void TestInconclusive(string message)
        {
            Assert.Inconclusive(message);
        }

        public void TestIgnore(string message)
        {
            Assert.Ignore(message);
        }

        public bool DelayedFixtureTearDown => false;
    }
}