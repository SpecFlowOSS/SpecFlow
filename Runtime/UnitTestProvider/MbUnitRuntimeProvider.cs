using System;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class MbUnitRuntimeProvider : IUnitTestRuntimeProvider
    {
        private const string MSTEST_ASSEMBLY = "MbUnit";
        private const string ASSERT_TYPE = "MbUnit.Framework.Assert";

        private Action<string, object[]> assertInconclusive;

        public virtual void TestPending(string message)
        {
            TestInconclusive(message);
        }

        public virtual void TestInconclusive(string message)
        {
            if (assertInconclusive == null)
            {
                assertInconclusive = UnitTestRuntimeProviderHelper
                    .GetAssertMethodWithFormattedMessage(MSTEST_ASSEMBLY, ASSERT_TYPE, "Inconclusive");
            }

            assertInconclusive("{0}", new object[] { message });
        }

        public virtual void TestIgnore(string message)
        {
            TestInconclusive(message); // there is no dynamic "Ignore" in mstest
        }

        public virtual bool DelayedFixtureTearDown
        {
            get { return true; }
        }
    }
}