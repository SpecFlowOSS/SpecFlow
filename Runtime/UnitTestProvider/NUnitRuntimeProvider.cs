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

        public void TestInconclusive(string message)
        {
            if (assertInconclusive == null)
            {
                try
                {
                    assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethod(
                        NUNIT_ASSEMBLY, ASSERT_TYPE, "Inconclusive");
                }
                catch(SpecFlowException)
                {
                    // for earlier nunit versions, where the Inconclusive was not supported yet
                    assertInconclusive = UnitTestRuntimeProviderHelper.GetAssertMethod(
                        NUNIT_ASSEMBLY, ASSERT_TYPE, "Ignore");
                }
            }

            assertInconclusive(message, new object[0]);
        }

        public void TestIgnore(string message)
        {
            if (assertIgnore == null)
            {
                assertIgnore = UnitTestRuntimeProviderHelper.GetAssertMethod(NUNIT_ASSEMBLY, ASSERT_TYPE, "Ignore");
            }

            assertIgnore(message, new object[0]);
        }

        public bool DelayedFixtureTearDown
        {
            get { return false; }
        }
    }
}