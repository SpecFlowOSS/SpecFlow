using System;
using System.Reflection;
using Moq;
using NUnit.Framework;
using ParserTests;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class SuccessfulExecutionTest : ExecutionTestBase
    {
        [SetUp]
        public void FixtureSetup()
        {
            var testRunner = TestTestRunnerFactory.CreateTestRunner();
            testRunner.InitializeTestRunner(new Assembly[0]); // no bindings

            var testRunnerManagerStub = new Mock<ITestRunnerManager>();
            testRunnerManagerStub.Setup(trm => trm.GetTestRunner(It.IsAny<Assembly>(), It.IsAny<bool>())).Returns(testRunner);

            TestRunnerManager.Instance = testRunnerManagerStub.Object;
        }

        [TearDown]
        public void TearDown()
        {
            TestRunnerManager.Reset();
        }

        [Test, TestCaseSource(typeof(TestFileHelper), "GetTestFiles")]
        public void CanExecuteForFile(string fileName)
        {
            ExecuteForFile(fileName);
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
