using System;
using System.Linq;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public interface IUnitTestRuntimeProvider
    {
        void TestPending(string message);
        void TestInconclusive(string message);
        void TestIgnore(string message);
        bool DelayedFixtureTearDown { get; }
    }
}