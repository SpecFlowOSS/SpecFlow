using System;

using NUnit.Framework;

using Rhino.Mocks;

using TechTalk.SpecFlow.Compatibility;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class AsyncTestRunnerTest
    {
        private const string Text = "text";
        private const string MultilineTextArg = "multilineTextArg";

        private readonly Table table = new Table("sausages");

        private ITestRunner mockTestRunner;
        private FakeAsyncContextImpl fakeAsyncContextImpl;
        private AsyncTestRunner asyncTestRunner;

        [SetUp]
        public void SetUp()
        {
            mockTestRunner = MockRepository.GenerateMock<ITestRunner>();
            fakeAsyncContextImpl = new FakeAsyncContextImpl();
            AsyncContext.Register(fakeAsyncContextImpl);

            asyncTestRunner = new AsyncTestRunner(mockTestRunner);
        }

        [TearDown]
        public void TearDown()
        {
            ObjectContainer.AsyncContext = null;
        }

        [Test]
        public void OnFeatureStartDefersToInnerSynchronousTestRunner()
        {
            var featureInfo = new FeatureInfo(CultureInfoHelper.GetCultureInfo("en-us"), "title", "description");

            asyncTestRunner.OnFeatureStart(featureInfo);
            
            mockTestRunner.AssertWasCalled(m => m.OnFeatureStart(featureInfo));
        }

        [Test]
        public void SetsCurrentTestRunnerToSelfOnFeatureStart()
        {
            var featureInfo = new FeatureInfo(CultureInfoHelper.GetCultureInfo("en-us"), "title", "description");

            asyncTestRunner.OnFeatureStart(featureInfo);

            Assert.IsNotNull(ObjectContainer.CurrentTestRunner);
            Assert.AreSame(asyncTestRunner, ObjectContainer.CurrentTestRunner);
        }

        [Test]
        public void OnFeatureEndDefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.OnFeatureEnd();

            mockTestRunner.AssertWasCalled(m => m.OnFeatureEnd());
        }

        [Test]
        public void ClearsCurrentTestRunnerOnFeatureEnd()
        {
            asyncTestRunner.OnFeatureEnd();

            Assert.IsNull(ObjectContainer.CurrentTestRunner);
        }

        [Test]
        public void OnScenarioStartThrowsIfAsyncContextNotSet()
        {
            ObjectContainer.AsyncContext = null;

            try
            {
                asyncTestRunner.OnScenarioStart(new ScenarioInfo("title"));
                Assert.Fail("Expected OnScenarioStart to fail if AsyncContext not set");
            }
            catch (InvalidOperationException e)
            {
                // Do nothing
            }
        }

        [Test]
        public void OnScenarioStartDefersToInnerSynchronousTestRunner()
        {
            var scenarioInfo = new ScenarioInfo("title");
            asyncTestRunner.OnScenarioStart(scenarioInfo);

            mockTestRunner.AssertWasCalled(m => m.OnScenarioStart(scenarioInfo));
        }

        [Test]
        public void OnScenarioEndDefersToInnerSynchronousTestRunner()
        {
            asyncTestRunner.OnScenarioEnd();

            mockTestRunner.AssertWasCalled(m => m.OnScenarioEnd());
        }

        [Test]
        public void OnScenarioEndClearsAsyncContext()
        {
            AsyncContext.Register(new FakeAsyncContextImpl());
            Assert.IsNotNull(ObjectContainer.AsyncContext);

            asyncTestRunner.OnScenarioEnd();

            Assert.IsNull(ObjectContainer.AsyncContext);
            Assert.IsNull(AsyncContext.Current);
        }

        [Test]
        public void GivenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {

            asyncTestRunner.Given(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.Given(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncContextImpl.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.Given(Text, MultilineTextArg, table));
        }

        [Test]
        public void WhenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.When(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.When(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncContextImpl.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.When(Text, MultilineTextArg, table));
        }

        [Test]
        public void ThenEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Then(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.Then(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncContextImpl.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.Then(Text, MultilineTextArg, table));
        }

        [Test]
        public void AndEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.And(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.And(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncContextImpl.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.And(Text, MultilineTextArg, table));
        }

        [Test]
        public void ButEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.But(Text, MultilineTextArg, table);

            mockTestRunner.AssertWasNotCalled(m => m.But(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Table>.Is.Anything));

            fakeAsyncContextImpl.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.But(Text, MultilineTextArg, table));
        }

        [Test]
        public void PendingEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.Pending();

            mockTestRunner.AssertWasNotCalled(m => m.Pending());

            fakeAsyncContextImpl.EnqueuedWithNewContext();

            mockTestRunner.AssertWasCalled(m => m.Pending());
        }

        [Test]
        public void CollectScenarioErrorsEnqueuesInnerSynchronousTestRunnerWithNewContext()
        {
            asyncTestRunner.CollectScenarioErrors();

            mockTestRunner.AssertWasNotCalled(m => m.CollectScenarioErrors());

            fakeAsyncContextImpl.EnqueuedCallback();

            mockTestRunner.AssertWasCalled(m => m.CollectScenarioErrors());
        }

        [Test]
        public void CollectScenarioErrorsEnqueuesTestCompleteAsLastOperation()
        {
            asyncTestRunner.CollectScenarioErrors();

            fakeAsyncContextImpl.EnqueuedCallback();
            Assert.IsTrue(fakeAsyncContextImpl.TestComplete);
        }

        private class FakeAsyncContextImpl : IAsyncContextImpl
        {
            public bool TestComplete;
            public Action EnqueuedWithNewContext;
            public Action EnqueuedCallback;

            public void EnqueueDelay(TimeSpan delay)
            {
                throw new NotImplementedException();
            }

            public void EnqueueConditional(Func<bool> continueUntil)
            {
                throw new NotImplementedException();
            }

            public void EnqueueCallback(Action callback)
            {
                EnqueuedCallback = callback;
            }

            public void EnqueueCallback(params Action[] callbacks)
            {
                throw new NotImplementedException();
            }

            public void EnqueueWithNewContext(Action action)
            {
                EnqueuedWithNewContext = action;
            }

            public void EnqueueTestComplete()
            {
                TestComplete = true;
            }
        }
    }
}