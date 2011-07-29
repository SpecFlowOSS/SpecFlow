using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    [TestFixture]
    public class AsyncTestRunnerTest
    {
        private const string Text = "text";
        private const string MultilineTextArg = "multilineTextArg";

        private readonly Table table = new Table("sausages");

        private ITestRunner mockTestRunner;
        private FakeAsyncTestExecutor fakeAsyncTestExecutor;
        private AsyncTestRunner asyncTestRunner;

        [SetUp]
        public void SetUp()
        {
            mockTestRunner = MockRepository.GenerateMock<ITestRunner>();
            fakeAsyncTestExecutor = new FakeAsyncTestExecutor();

            asyncTestRunner = new AsyncTestRunner(mockTestRunner);
            asyncTestRunner.RegisterAsyncTestExecutor(fakeAsyncTestExecutor);
            var scenarioInfo = new ScenarioInfo("sample scenario");
            ObjectContainer.ScenarioContext = new ScenarioContext(scenarioInfo, mockTestRunner);
            asyncTestRunner.OnScenarioStart(scenarioInfo);
        }

        [TearDown]
        public void TearDown()
        {
            asyncTestRunner.OnScenarioEnd();
        }

        [Test]
        public void GivenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Given(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.Given(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.Given(Text, MultilineTextArg, table));
        }

        [Test]
        public void WhenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.When(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.When(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.When(Text, MultilineTextArg, table));
        }

        [Test]
        public void ThenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Then(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.Then(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.Then(Text, MultilineTextArg, table));
        }

        [Test]
        public void AndEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.And(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.And(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.And(Text, MultilineTextArg, table));
        }

        [Test]
        public void ButEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.But(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.But(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.But(Text, MultilineTextArg, table));
        }

        [Test]
        public void PendingEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Pending();

            mockTestRunner.AssertWasNotCalled(m => m.Pending());

            fakeAsyncTestExecutor.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.Pending());
        }

        [Test]
        public void CollectScenarioErrorsEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.CollectScenarioErrors();

            mockTestRunner.AssertWasNotCalled(m => m.CollectScenarioErrors());

            fakeAsyncTestExecutor.EnqueuedCallback();

            mockTestRunner.AssertWasCalled(m => m.CollectScenarioErrors());
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