using System;
using System.Linq;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class NUnitRuntimeProvider : IUnitTestRuntimeProvider
    {
        private const string NUNIT_ASSEMBLY = "nunit.framework";
        private const string ASSERT_TYPE = "NUnit.Framework.Assert";

        Action<string, object[]> assertInconclusive = null;
        Action<string, object[]> assertIgnore = null;

        public void TestPending(string message)
        {
            TestInconclusive(message);
        }

        public void TestInconclusive(string message)
        {
            if (assertInconclusive == null)
            {
                try
                {
                    assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethodWithFormattedMessage(
                        NUNIT_ASSEMBLY, ASSERT_TYPE, "Inconclusive");
                }
                catch(SpecFlowException)
                {
                    // for earlier nunit versions, where the Inconclusive was not supported yet
                    assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethodWithFormattedMessage(
                        NUNIT_ASSEMBLY, ASSERT_TYPE, "Ignore");
                }
            }

            assertInconclusive("{0}", new object[] { message });
        }

        public void TestIgnore(string message)
        {
            if (assertIgnore == null)
            {
                assertIgnore = UnitTestRuntimeProviderHelper.GetAssertMethodWithFormattedMessage(NUNIT_ASSEMBLY, ASSERT_TYPE, "Ignore");
            }

            assertIgnore("{0}", new object[] { message });
        }

        public bool DelayedFixtureTearDown
        {
            get { return false; }
        }
    }
}