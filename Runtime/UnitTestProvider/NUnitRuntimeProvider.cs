using System;
using System.Linq;
using NUnit.Framework;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class NUnitRuntimeProvider : IUnitTestRuntimeProvider
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