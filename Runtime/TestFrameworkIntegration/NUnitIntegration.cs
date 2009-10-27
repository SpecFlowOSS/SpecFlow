using System;
using System.Linq;
using NUnit.Framework;

namespace TechTalk.SpecFlow.TestFrameworkIntegration
{
    public class NUnitIntegration : ITestFrameworkIntegration
    {
        public void TestInconclusive(string message)
        {
            Assert.Inconclusive(message);
        }

        public void TestIgnore(string message)
        {
            Assert.Ignore(message);
        }
    }
}