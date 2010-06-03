using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace ParserTests.TestFrameworks
{
    [TestFixture]
    public class NUnitTestActivator
    {
        private const string TESTFIXTURE_ATTR = "NUnit.Framework.TestFixtureAttribute";
        private const string TEST_ATTR = "NUnit.Framework.TestAttribute";
        private const string CATEGORY_ATTR = "NUnit.Framework.CategoryAttribute";
        private const string TESTSETUP_ATTR = "NUnit.Framework.SetUpAttribute";
        private const string TESTFIXTURESETUP_ATTR = "NUnit.Framework.TestFixtureSetUpAttribute";
        private const string TESTFIXTURETEARDOWN_ATTR = "NUnit.Framework.TestFixtureTearDownAttribute";
        private const string TESTTEARDOWN_ATTR = "NUnit.Framework.TearDownAttribute";
        private const string IGNORE_ATTR = "NUnit.Framework.IgnoreAttribute";
        private const string DESCRIPTION_ATTR = "NUnit.Framework.DescriptionAttribute";
        
        [Test]
        public void CanGenerateButFeature()
        {
            Assert.IsNotNull(Type.GetType(TESTFIXTURE_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(TEST_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(CATEGORY_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(TESTSETUP_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(TESTFIXTURESETUP_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(TESTFIXTURETEARDOWN_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(TESTTEARDOWN_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(IGNORE_ATTR + ",nunit.framework"));
            Assert.IsNotNull(Type.GetType(DESCRIPTION_ATTR + ",nunit.framework"));
        }
    }
}
