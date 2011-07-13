using System;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow
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

        /// <summary>
        /// Retrieve the current <see cref="AsyncContext"/>. Will be null unless the
        /// generator and runtime support asynchronous tests.
        /// </summary>
        public static AsyncContext Current
        {
            //get { return ObjectContainer.AsyncContext; }
            get
            {
                throw new NotImplementedException(); //TODO
            }
        }

        private AsyncContext(IAsyncTestExecutor asyncTestExecutor)
        {
            this.asyncTestExecutor = asyncTestExecutor;
        }

        /// <summary>
        /// Intended for framework use.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of <see cref="AsyncContext"/> using the given
        /// <see cref="IAsyncTestExecutor"/> as its implementation. Intended to be
        /// called from the generated test code.
        /// </remarks>
        public static void Register(IAsyncTestExecutor asyncTestExecutor)
        {
            var asyncContext = new AsyncContext(asyncTestExecutor);
            //ObjectContainer.AsyncContext = asyncContext;
            throw new NotImplementedException(); //TODO
        }

        internal static void Unregister()
        {
            //ObjectContainer.AsyncContext = null;
            throw new NotImplementedException(); //TODO
        }

        internal void EnqueueWithNewContext(Action action)
        {
            asyncTestExecutor.EnqueueWithNewContext(action);
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

        internal void EnqueueTestComplete()
        {
            asyncTestExecutor.EnqueueTestComplete();
        }
    }
}