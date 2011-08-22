using Moq;
using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    [TestFixture]
    public class AsyncTestRunnerTest : AsyncTestRunnerTestBase
    {
        private const string Text = "text";
        private const string MultilineTextArg = "multilineTextArg";

        private readonly Table table = new Table("sausages");

        public override void SetUp()
        {
            base.SetUp();

            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);

            var scenarioInfo = new ScenarioInfo("sample scenario");
            asyncTestRunner.OnScenarioStart(scenarioInfo);
        }

        [Test]
        public void GivenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Given(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.Given(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.Verify(m => m.Given(Text, MultilineTextArg, table));
        }

        [Test]
        public void WhenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.When(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.When(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.Verify(m => m.When(Text, MultilineTextArg, table));
        }

        [Test]
        public void ThenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Then(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.Then(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.Verify(m => m.Then(Text, MultilineTextArg, table));
        }

        [Test]
        public void AndEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.And(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.And(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.Verify(m => m.And(Text, MultilineTextArg, table));
        }

        [Test]
        public void ButEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.But(Text, MultilineTextArg, table);

            mockTestRunner.Verify(m => m.But(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.Verify(m => m.But(Text, MultilineTextArg, table));
        }

        [Test]
        public void PendingEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Pending();

            mockTestRunner.Verify(m => m.Pending(), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.Verify(m => m.Pending());
        }

        [Test]
        public void CollectScenarioErrorsEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.CollectScenarioErrors();

            mockTestRunner.Verify(m => m.CollectScenarioErrors(), Times.Never());

            fakeAsyncTestExecutor.EnqueuedCallback();

            mockTestRunner.Verify(m => m.CollectScenarioErrors());
        }

        [Test]
        public void CollectScenarioErrorsEnqueuesTestCompleteAsLastOperation()
        {
            asyncTestRunner.CollectScenarioErrors();

            fakeAsyncTestExecutor.EnqueuedCallback();
            Assert.IsTrue(fakeAsyncTestExecutor.TestComplete);
        }
    }
}