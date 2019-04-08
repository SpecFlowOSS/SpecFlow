using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Bindings
{
    /// <summary>
    /// https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously#answer-5097066
    /// </summary>
    public static class AsyncHelpers
    {
        /// <summary>
        /// Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            var synch = new ExclusiveSynchronizationContext();
            using (new SetSynchronizationContext(synch))
            {
                synch.Post(async _ =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        synch.InnerExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                        throw;
                    }
                    finally
                    {
                        synch.EndMessageLoop();
                    }
                }, null);
                synch.BeginMessageLoop();
            }
        }

        /// <summary>
        /// Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var synch = new ExclusiveSynchronizationContext();
            using (new SetSynchronizationContext(synch))
            {
                T ret = default(T);
                synch.Post(async _ =>
                {
                    try
                    {
                        ret = await task();
                    }
                    catch (Exception e)
                    {
                        synch.InnerExceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                        throw;
                    }
                    finally
                    {
                        synch.EndMessageLoop();
                    }
                }, null);
                synch.BeginMessageLoop();
                return ret;
            }
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public ExceptionDispatchInfo InnerExceptionDispatchInfo { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerExceptionDispatchInfo != null) // the method threw an exeption
                        {
                            InnerExceptionDispatchInfo.Throw();
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }

        private sealed class SetSynchronizationContext: IDisposable
        {
            private SynchronizationContext _oldContext;

            public SetSynchronizationContext(SynchronizationContext synchronizationContext)
            {
                _oldContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            }

            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_oldContext);
            }
        }
    }
}
