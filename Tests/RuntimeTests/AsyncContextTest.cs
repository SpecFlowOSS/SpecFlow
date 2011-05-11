using System;

using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class AsyncContextTest
    {
        private static FakeAsyncContextImpl fakeAsyncContextImpl;

        [TearDown]
        public void TearDown()
        {
            ObjectContainer.AsyncContext = null;
        }

        [Test]
        public void AsyncContextIsNullIfNothingIsRegistered()
        {
            Assert.IsNull(AsyncContext.Current);
        }

        private static void SetupAsyncContext()
        {
            fakeAsyncContextImpl = new FakeAsyncContextImpl();

            AsyncContext.Register(fakeAsyncContextImpl);
        }

        [Test]
        public void AsyncContextIsStoredOnObjectContainer()
        {
            SetupAsyncContext();
            Assert.IsNotNull(AsyncContext.Current);
            Assert.IsInstanceOf<AsyncContext>(AsyncContext.Current);

            Assert.IsNotNull(ObjectContainer.AsyncContext);
            Assert.AreSame(AsyncContext.Current, ObjectContainer.AsyncContext);

            ObjectContainer.AsyncContext = null;

            Assert.IsNull(AsyncContext.Current);
        }

        [Test]
        public void EnqueueWithNewContextIsDeferredToImpl()
        {
            Action action = () => { };

            SetupAsyncContext();
            AsyncContext.Current.EnqueueWithNewContext(action);

            Assert.IsNotNull(fakeAsyncContextImpl.EnqueuedWithNewContextAction);
            Assert.AreSame(action, fakeAsyncContextImpl.EnqueuedWithNewContextAction);
        }

        [Test]
        public void EnqueueCallbackIsDeferredToImpl()
        {
            Action action = () => { };

            SetupAsyncContext();
            AsyncContext.Current.EnqueueCallback(action);

            Assert.IsNotNull(fakeAsyncContextImpl.EnqueuedAction);
            Assert.AreSame(action, fakeAsyncContextImpl.EnqueuedAction);
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

            Assert.IsNotNull(fakeAsyncContextImpl.EnqueuedActions);
            Assert.AreSame(actions, fakeAsyncContextImpl.EnqueuedActions);
        }

        [Test]
        public void EnqueueConditionalIsDeferredToImpl()
        {
            Func<bool> predicate = () => true;

            SetupAsyncContext();
            AsyncContext.Current.EnqueueConditional(predicate);

            Assert.IsNotNull(fakeAsyncContextImpl.EnqueuedConditional);
            Assert.AreSame(predicate, fakeAsyncContextImpl.EnqueuedConditional);
        }

        [Test]
        public void EnqueueDelayIsDeferredToImpl()
        {
            const int delay = 1000;

            SetupAsyncContext();
            AsyncContext.Current.EnqueueDelay(delay);

            Assert.AreEqual(TimeSpan.FromMilliseconds(delay), fakeAsyncContextImpl.EnqueuedDelay);
        }

        [Test]
        public void EnqueueTestCompleteIsDeferredToImpl()
        {
            SetupAsyncContext();
            AsyncContext.Current.EnqueueTestComplete();

            Assert.IsTrue(fakeAsyncContextImpl.EnqueuedTestComplete);
        }

        private class FakeAsyncContextImpl : IAsyncContextImpl
        {
            public TimeSpan EnqueuedDelay;
            public Action EnqueuedAction;
            public Action[] EnqueuedActions;
            public Func<bool> EnqueuedConditional;
            public Action EnqueuedWithNewContextAction;
            public bool EnqueuedTestComplete;

            public void EnqueueDelay(TimeSpan delay)
            {
                EnqueuedDelay = delay;
            }

            public void EnqueueConditional(Func<bool> continueUntil)
            {
                EnqueuedConditional = continueUntil;
            }

            public void EnqueueCallback(Action callback)
            {
                EnqueuedAction = callback;
            }

            public void EnqueueCallback(params Action[] callbacks)
            {
                EnqueuedActions = callbacks;
            }

            public void EnqueueWithNewContext(Action action)
            {
                EnqueuedWithNewContextAction = action;
            }

            public void EnqueueTestComplete()
            {
                EnqueuedTestComplete = true;
            }
        }
    }
}