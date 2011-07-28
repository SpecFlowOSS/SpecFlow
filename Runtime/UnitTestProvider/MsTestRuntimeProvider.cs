using System;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class MsTestRuntimeProvider : IUnitTestRuntimeProvider
    {
        private const string MSTEST_ASSEMBLY = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";

        private const string ASSERT_TYPE = "Microsoft.VisualStudio.TestTools.UnitTesting.Assert";

        Action<string, object[]> assertInconclusive = null;
        
        public virtual string AssemblyName
        {
            get { return MSTEST_ASSEMBLY; }
        }

        public void TestPending(string message)
        {
            TestInconclusive(message);
        }

        public void TestInconclusive(string message)
        {
            if (assertInconclusive == null)
            {
                assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethodWithFormattedMessage(AssemblyName, ASSERT_TYPE, "Inconclusive");
            }

            assertInconclusive("{0}", new object[] { message });
        }

        public void TestIgnore(string message)
        {
            TestInconclusive(message); // there is no dynamic "Ignore" in mstest
        }

        public bool DelayedFixtureTearDown
        {
            get { return true; }
        }

        
    }
}