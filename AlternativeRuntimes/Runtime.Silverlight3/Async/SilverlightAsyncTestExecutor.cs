using System;
using System.Reflection;

namespace TechTalk.SpecFlow.Async
{
    /// <summary>
    /// Provides an implementation of <see cref="IAsyncTestExecutor"/> using the Silverlight
    /// Unit Testing Framework's asynchronous support.
    /// </summary>
    public class SilverlightAsyncTestExecutor : IAsyncTestExecutor
    {
        private readonly ISilverlightTestInstance silverlightTestInstance;
        private readonly UnitTestingFrameworkHelper unitTestingFrameworkHelper;

        public SilverlightAsyncTestExecutor(ISilverlightTestInstance silverlightTestInstance)
        {
            this.silverlightTestInstance = silverlightTestInstance;

            unitTestingFrameworkHelper = new UnitTestingFrameworkHelper(silverlightTestInstance);
        }

        public void EnqueueDelay(TimeSpan delay)
        {
            silverlightTestInstance.EnqueueDelay(delay);
        }

        public void EnqueueConditional(Func<bool> continueUntil)
        {
            silverlightTestInstance.EnqueueConditional(continueUntil);
        }

        public void EnqueueCallback(Action callback)
        {
            silverlightTestInstance.EnqueueCallback(callback);
        }

        public void EnqueueCallback(params Action[] callbacks)
        {
            silverlightTestInstance.EnqueueCallback(callbacks);
        }

        public void EnqueueWithNewContext(Action action)
        {
            // The Silverlight testing framework uses a work item pattern to run tasks. The root
            // work item is a composite, and is owned by the work item manager. When you enqueue
            // something, it gets added as a new work item to the end of the current composite work item.
            // So, imagine I have a root composite item X. It has one child, { A }. When I run A, it enqueues
            // work items A1, A2 and A3. X now has 4 children - { A, A1, A2, A3 }. Once A is finished, X
            // will run A1. If A1 enqueues A1a, X now has 5 children, { A, A1, A2, A3, A1a }.
            // If I want A1 to enqueue something that runs before A2, I need A1 to actually be a composite,
            // i.e. X contains { A, Y, A2, A3 } - where Y is a composite containing { A1 }
            // If A1 enqueues into Y, I'd get { A, Y = {A1, A1a, A1b}, A2, A3 }
            // But to get A1 to enqueue into Y using the normal enqueue functions, I need to push Y onto the
            // stack owned by the work item manager, and then pop it again once it's empty:
            // A, push(Y), Y = {A1, A1a, A1b}, Y(pop), A2, A3
            //
            // So that's what we're doing here:
            // 1. Create a composite (Y)
            // 2. Enqueue a push as the default work item (enqueued so currently queued work items will execute first)
            // 3. Enqueue the action (calling Enqueue from within the action will enqueue into the pushed composite)
            // 4. Enqueue the composite to run after the action (so it's work queue will get executed)
            // 5. Enqueue a pop of the enqueued composite (Y) (enqueued in the current work item, not the new one)
            // 6. Rethrow any unhandled exceptions captured by the composite
            //
            // Easy, eh?

            Exception unhandledException = null;
            var compositeWorkItem = unitTestingFrameworkHelper.CreateCompositeWorkItem(ex => unhandledException = ex);

            EnqueueCallback(() =>
                            {
                                unitTestingFrameworkHelper.PushDefaultWorkItem(compositeWorkItem);
                                action();
                            });
            unitTestingFrameworkHelper.EnqueueWorkItem(compositeWorkItem);
            EnqueueCallback(() => unitTestingFrameworkHelper.PopDefaultWorkItem());
            EnqueueCallback(() =>
                            {
                                if (unhandledException != null)
                                {
                                    // We want to maintain the call stack of the exception. If it's a TargetInvocationException,
                                    // the silverlight testing framework will unwrap it for us, preserving the stack, so either
                                    // wrap it up, or rethrow an existing instance.
                                    // The thrown exception will get caught by the owning WorkItem, which might be a composite
                                    // we've enqueued, so it will get rethrown and eventually caught by the framework
                                    if (unhandledException is TargetInvocationException)
                                        throw unhandledException;
                                    throw new TargetInvocationException(unhandledException);
                                }
                            });
        }

        public void EnqueueTestComplete()
        {
            silverlightTestInstance.EnqueueTestComplete();
        }

        public void Dispose()
        {
            //nop;
        }
    }
}