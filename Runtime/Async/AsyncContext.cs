using System;

namespace TechTalk.SpecFlow.Async
{
    /// <summary>
    /// Provides the entry point to enqueue tasks to run asynchronously
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the generator and runtime provide support for asynchronous behaviour, then each step will be
    /// enqueued to run asynchronously. If the step is simple, no code changes are required. To wait
    /// until a previously started async operation completes (e.g. a web request), use <see cref="EnqueueConditional"/>
    /// providing a conditional predicate that will return true once the operation has completed. To
    /// wait a certain amount of time before continuing, use <see cref="EnqueueDelay(TimeSpan)"/>. And
    /// to enqueue more actions, use <see cref="EnqueueCallback(System.Action)"/>.
    /// </para>
    /// </remarks>
    public class AsyncContext
    {
        private readonly IAsyncTestExecutor asyncTestExecutor;

        public AsyncContext(IAsyncTestExecutor asyncTestExecutor)
        {
            this.asyncTestExecutor = asyncTestExecutor;
        }

        /// <summary>
        /// Retrieve the current <see cref="AsyncContext"/>. Will be null unless the
        /// generator and runtime support asynchronous tests.
        /// </summary>
        public static AsyncContext Current
        {
            get
            {
                AsyncContext current;
                if (!ScenarioContext.Current.TryGetValue(out current))
                {
                    IAsyncTestExecutor testExecutor;
                    if (!ScenarioContext.Current.TryGetValue(out testExecutor))
                        return null;

                    current = new AsyncContext(testExecutor);
                    ScenarioContext.Current.Set(current);
                }
                return current;
            }
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