using System;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.RuntimeTests.AsyncTests
{
    public class FakeAsyncTestExecutor : IAsyncTestExecutor
    {
        public bool TestComplete;
        public Action EnqueuedWithNewContext;
        public Action EnqueuedCallback;
        public TimeSpan EnqueuedDelay;
        public Action[] EnqueuedCallbacks;
        public Func<bool> EnqueuedConditional;

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
            EnqueuedCallback = callback;
        }

        public void EnqueueCallback(params Action[] callbacks)
        {
            EnqueuedCallbacks = callbacks;
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