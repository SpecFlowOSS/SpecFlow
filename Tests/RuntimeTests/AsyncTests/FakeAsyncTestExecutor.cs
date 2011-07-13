using System;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.RuntimeTests
{
    internal class FakeAsyncTestExecutor : IAsyncTestExecutor
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

        public void Dispose()
        {
                
        }
    }
}