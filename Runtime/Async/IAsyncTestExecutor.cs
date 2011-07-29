using System;

namespace TechTalk.SpecFlow.Async
{
    /// <summary>
    /// Intended for framework use.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Intended to be implemented by test framework providers. Provides the implementation
    /// to actually perform the enqueueing of asynchronous operations.
    /// </para>
    /// <para>
    /// Purposely decouples the implementation of the enqueueing from the implementation of
    /// the <see cref="AsyncContext"/>. For example, in Silverlight, this interface is
    /// implemented by the feature test classes.
    /// </para>
    /// </remarks>
    public interface IAsyncTestExecutor : IDisposable
    {
        /// <summary>
        /// Enqueues an asynchronous and non-blocking wait for at least the given time before continuing.
        /// </summary>
        /// <remarks>
        /// The delay is approximate, and not intended to be highly accurate.
        /// </remarks>
        void EnqueueDelay(TimeSpan delay);

        /// <summary>
        /// Enqueues an asynchronous and non-blocking wait until the condition returns true.
        /// </summary>
        /// <param name="continueUntil">Predicate that must return true before the work queue is continued.</param>
        void EnqueueConditional(Func<bool> continueUntil);

        /// <summary>
        /// Enqueues an action to be executed asynchronously.
        /// </summary>
        /// <param name="callback"></param>
        void EnqueueCallback(Action callback);

        /// <summary>
        /// Enqueues a number of actions to be executed asynchronously, but consecutively.
        /// </summary>
        void EnqueueCallback(params Action[] callbacks);

        /// <summary>
        /// Creates a new asynchronous context and runs the action. Anything queued
        /// by this action will be enqueued within the new context and executed before
        /// continuing with the actions enqueued in the current context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The context is expected to maintain a list (queue) of work items (callbacks,
        /// conditionals or delays) that are executed sequentially, but asynchronously.
        /// The test framework's async support is expected to run this list of work items.
        /// When a scenario is executed, the <see cref="AsyncTestRunner"/> will enqueue
        /// each step using <see cref="EnqueueWithNewContext"/>. If the step wishes to
        /// enqueue further actions (such as calling <see cref="Steps.Given(string)"/>
        /// or <see cref="Steps.When(string)"/>, etc) these actions must run before the
        /// next, already-enqueued step.
        /// </para>
        /// <para>
        /// So, each step is enqueued with a new context. When that step enqueues further
        /// actions, they get added to the end of the new context and are all executed
        /// before the previous context is continued.
        /// </para>
        /// <para>
        /// It is expected that the previous context is restored at the appropriate
        /// time by the derived class, probably in conjunction with the test framework.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to execute in the new context</param>
        void EnqueueWithNewContext(Action action);

        /// <summary>
        /// Enqueues the test complete marker. Intended for internal framework use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Asynchronous tests will complete some time after the initial test method
        /// has completed. This method is expected to add an item to the end of the
        /// internal queue of actions that will notify the test framework that the
        /// tests are complete.
        /// </para>
        /// <para>
        /// Used by the <see cref="AsyncTestRunner"/> during <see cref="ITestRunner.CollectScenarioErrors"/>.
        /// Marked as internal abstract rather than implemented on <see cref="IAsyncTestExecutor"/>
        /// because <see cref="IAsyncTestExecutor"/> is public (so others can implement it) and
        /// we don't want people calling it unexpectedly.
        /// </para>
        /// </remarks>
        void EnqueueTestComplete();
    }
}