using System;
using System.Linq;
using BoDi;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public interface IUnitTestRuntimeProvider
    {
        void TestPending(string message);
        void TestInconclusive(string message);
        void TestIgnore(string message);
        bool DelayedFixtureTearDown { get; }
        void RegisterContextManagers(IObjectContainer objectContainer);
    }
}