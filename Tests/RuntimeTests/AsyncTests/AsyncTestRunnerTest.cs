using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;

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

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.Given, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.Given, Text, MultilineTextArg, table));
        }

        [Test]
        public void WhenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.When(Text, MultilineTextArg, table);

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.When, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.When, Text, MultilineTextArg, table));
        }

        [Test]
        public void ThenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Then(Text, MultilineTextArg, table);

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.Then, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.Then, Text, MultilineTextArg, table));
        }

        [Test]
        public void AndEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.And(Text, MultilineTextArg, table);

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.And, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.And, Text, MultilineTextArg, table));
        }

        [Test]
        public void ButEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.But(Text, MultilineTextArg, table);

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.But, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Table>()), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            testExecutionEngineStub.Verify(m => m.Step(StepDefinitionKeyword.But, Text, MultilineTextArg, table));
        }

        [Test]
        public void PendingEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Pending();

            testExecutionEngineStub.Verify(m => m.Pending(), Times.Never());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            testExecutionEngineStub.Verify(m => m.Pending());
        }

        [Test]
        public void CollectScenarioErrorsEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.CollectScenarioErrors();

            testExecutionEngineStub.Verify(m => m.OnAfterLastStep(), Times.Never());

            fakeAsyncTestExecutor.EnqueuedCallback();

            testExecutionEngineStub.Verify(m => m.OnAfterLastStep());
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