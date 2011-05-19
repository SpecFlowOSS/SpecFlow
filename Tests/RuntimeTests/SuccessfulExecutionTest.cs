using System;
using System.Reflection;
using NUnit.Framework;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class SuccessfulExecutionTest : ExecutionTestBase
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var testRunner = new TestRunner();
            testRunner.InitializeTestRunner(new Assembly[0]); // no bindings
            ObjectContainer.CurrentTestRunner = testRunner;
        }

        protected override void ExecuteTests(object test, Feature feature)
        {
            NUnitTestExecutor.ExecuteNUnitTests(test,
                delegate(Exception exception)
                {
                    Assert.IsInstanceOf(typeof(InconclusiveException), exception);
                    return true;
                });
        }
    }
}
