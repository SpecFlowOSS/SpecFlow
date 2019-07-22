using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class XUnitRuntimeProvider : IUnitTestRuntimeProvider
    {
        public void TestPending(string message)
        {
            throw new XUnitPendingStepException($"Test pending: {message}");
        }

        public void TestInconclusive(string message)
        {
            throw new XUnitInconclusiveException("Test inconclusive: " + message);
        }

        public void TestIgnore(string message)
        {
            throw new XUnitIgnoreTestException("Test ignored: " + message);
        }

        public bool DelayedFixtureTearDown => false;
    }
}