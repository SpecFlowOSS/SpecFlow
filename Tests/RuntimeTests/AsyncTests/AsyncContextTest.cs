using System;
using BoDi;
using NUnit.Framework;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    [TestFixture]
    public class AsyncContextTest
    {
        private static FakeAsyncTestExecutor fakeAsyncTestExecutor;

        [SetUp]
        public void Setup()
        {
            IObjectContainer container;
            var testRunner = TestTestRunnerFactory.CreateTestRunner(out container);
            ScenarioContext.Current = new ScenarioContext(new ScenarioInfo("sample scenario"), testRunner, container);
        }

        [Test]
        public void AsyncContextIsNullIfNothingIsRegistered()
        {
            Assert.IsNull(AsyncContext.Current);
        }

        private static void SetupAsyncContext()
        {
            fakeAsyncTestExecutor = new FakeAsyncTestExecutor();
            ScenarioContext.Current.Set<IAsyncTestExecutor>(fakeAsyncTestExecutor);
        }

        [Test]
        public void AsyncContextIsStoredInScenarioContext()
        {
            SetupAsyncContext();
            Assert.IsNotNull(AsyncContext.Current);
            Assert.IsInstanceOf<AsyncContext>(AsyncContext.Current);

            Assert.IsNotNull(ScenarioContext.Current.Get<AsyncContext>());
            Assert.AreSame(AsyncContext.Current, ScenarioContext.Current.Get<AsyncContext>());
        }

        [Test]
        public void EnqueueCallbackIsDeferredToImpl()
        {
            Action action = () => { };

            SetupAsyncContext();
            AsyncContext.Current.EnqueueCallback(action);

            Assert.IsNotNull(fakeAsyncTestExecutor.EnqueuedCallback);
            Assert.AreSame(action, fakeAsyncTestExecutor.EnqueuedCallback);
        }

        [Test]
        public void EnqueueMultipleCallbacksIsDeferredToImpl()
        {
            var actions = new Action[]
                          {
                              () => { },
                              () => { }
                          };

            SetupAsyncContext();
            AsyncContext.Current.EnqueueCallback(actions);

            Assert.IsNotNull(fakeAsyncTestExecutor.EnqueuedCallbacks);
            Assert.AreSame(actions, fakeAsyncTestExecutor.EnqueuedCallbacks);
        }

        [Test]
        public void EnqueueConditionalIsDeferredToImpl()
        {
            Func<bool> predicate = () => true;

            SetupAsyncContext();
            AsyncContext.Current.EnqueueConditional(predicate);

            Assert.IsNotNull(fakeAsyncTestExecutor.EnqueuedConditional);
            Assert.AreSame(predicate, fakeAsyncTestExecutor.EnqueuedConditional);
        }

        [Test]
        public void EnqueueDelayIsDeferredToImpl()
        {
            const int delay = 1000;

            SetupAsyncContext();
            AsyncContext.Current.EnqueueDelay(delay);

            Assert.AreEqual(TimeSpan.FromMilliseconds(delay), fakeAsyncTestExecutor.EnqueuedDelay);
        }
    }
}