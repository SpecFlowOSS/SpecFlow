using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.TestFrameworkIntegration
{
    public interface ITestFrameworkIntegration
    {
        void TestInconclusive(string message);
        void TestIgnore(string message);
    }
}