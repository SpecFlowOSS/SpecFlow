using System;
using NUnit.Framework;

namespace ParserTests.TestFrameworks
{
    [TestFixture]
    public class MbUnitTestActivator
    {
        private const string TESTFIXTURE_ATTR = "MbUnit.Framework.TestFixtureAttribute";
        private const string TEST_ATTR = "MbUnit.Framework.TestAttribute";
        private const string CATEGORY_ATTR = "MbUnit.Framework.CategoryAttribute";
        private const string TESTSETUP_ATTR = "MbUnit.Framework.SetUpAttribute";
        private const string TESTFIXTURESETUP_ATTR = "MbUnit.Framework.FixtureSetUpAttribute";
        private const string TESTFIXTURETEARDOWN_ATTR = "MbUnit.Framework.FixtureTearDownAttribute";
        private const string TESTTEARDOWN_ATTR = "MbUnit.Framework.TearDownAttribute";
        private const string IGNORE_ATTR = "MbUnit.Framework.IgnoreAttribute";
        private const string DESCRIPTION_ATTR = "MbUnit.Framework.DescriptionAttribute";

        private const string MSTEST_ASSEMBLY = "MbUnit";
        private const string ASSERT_TYPE = "MbUnit.Framework.Assert";

        [Test]
        public void CanGenerateButFeature()
        {
            Assert.IsNotNull(Type.GetType(TESTFIXTURE_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(TEST_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(CATEGORY_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(TESTSETUP_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(TESTFIXTURESETUP_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(TESTFIXTURETEARDOWN_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(TESTTEARDOWN_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(IGNORE_ATTR + ",MbUnit"));
            Assert.IsNotNull(Type.GetType(DESCRIPTION_ATTR + ",MbUnit"));

            Assert.IsNotNull(Type.GetType(ASSERT_TYPE + "," + MSTEST_ASSEMBLY));
        }
    }
}