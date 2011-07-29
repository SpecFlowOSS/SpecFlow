using System;
using System.Linq;

namespace TechTalk.SpecFlow.Async
{
    /// <summary>
    /// Provides a base class to enqueue step bindings asynchronously
    /// </summary>
    public abstract class AsyncSteps : Steps
    {
        protected readonly IAsyncTestExecutor asyncTestExecutor;

        protected AsyncSteps(ITestRunner testRunner, IAsyncTestExecutor asyncTestExecutor) : base(testRunner)
        {
            this.asyncTestExecutor = asyncTestExecutor;
        }

        protected AsyncSteps()
        {
            if (!ScenarioContext.Current.TryGetValue(out asyncTestExecutor))
                throw new SpecFlowException("The AsyncSteps base class can only be used for asynchronous tests. Consider setting <generator generateAsyncTests=\"true\"/> in the configuration.");
        }

        /// <summary>
        /// Enqueues an action to be executed asynchronously.
        /// </summary>
        /// <param name="callback"></param>
        public void EnqueueCallback(Action callback)
        {
            asyncTestExecutor.EnqueueCallback(callback);
        }

        /// <summary>
        /// Enqueues a number of actions to be executed asynchronously, but consecutively.
        /// </summary>
        public void EnqueueCallback(params Action[] callbacks)
        {
            asyncTestExecutor.EnqueueCallback(callbacks);
        }

        /// <summary>
        /// Enqueues an asynchronous and non-blocking wait until the condition returns true.
        /// </summary>
        /// <param name="continueUntil">Predicate that must return true before the work queue is continued.</param>
        public void EnqueueConditional(Func<bool> continueUntil)
        {
            asyncTestExecutor.EnqueueConditional(continueUntil);
        }

        /// <summary>
        /// Enqueues an asynchronous and non-blocking wait for at least the given time before continuing.
        /// </summary>
        /// <remarks>
        /// The delay is approximate, and not intended to be highly accurate.
        /// </remarks>
        public void EnqueueDelay(TimeSpan delay)
        {
            asyncTestExecutor.EnqueueDelay(delay);
        }

        /// <summary>
        /// Enqueues an asynchronous and non-blocking wait for at least the given time before continuing.
        /// </summary>
        /// <remarks>
        /// The delay is approximate, and not intended to be highly accurate.
        /// </remarks>
        public void EnqueueDelay(double milliseconds)
        {
            EnqueueDelay(TimeSpan.FromMilliseconds(milliseconds));
        }
    }
}
