using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class IdleTaskProcessingQueue : IDisposable
    {
        private readonly IIdeTracer tracer;
        private readonly Action<IGherkinProcessingTask> doTask;
        public bool FlushFirstTask { get; private set; }
        private readonly ConcurrentQueue<IGherkinProcessingTask> tasks = new ConcurrentQueue<IGherkinProcessingTask>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Thread thread = null;

        readonly EventWaitHandle itemsAvailableEvent = new ManualResetEvent(false);
        public TimeSpan IdleTime { get; private set; }

        public IdleTaskProcessingQueue(TimeSpan idleTime, bool flushFirstTask, IIdeTracer tracer, Action<IGherkinProcessingTask> doTask)
        {
            this.tracer = tracer;
            this.doTask = doTask;
            FlushFirstTask = flushFirstTask;
            IdleTime = idleTime;
        }

        public void Start()
        {
            thread = new Thread(() =>
                {
                    try
                    {
                        Worker();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                })
                {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal
                };
            thread.Start();            
        }

        private void Worker()
        {
            var cancellationToken = cancellationTokenSource.Token;
            DateTime lastProcessed = DateTime.MinValue;

            while (!cancellationToken.IsCancellationRequested)
            {
                // wait for an event
                itemsAvailableEvent.WaitOne();
                itemsAvailableEvent.Reset();
                cancellationToken.ThrowIfCancellationRequested();

                // if we need to delay the processing for an idle time...
                if (!FlushFirstTask || !IsFirstTask(lastProcessed))
                {
                    // we keep waiting for additional events until the idle timeout comes in
                    while (itemsAvailableEvent.WaitOne(IdleTime))
                    {
                        itemsAvailableEvent.Reset();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                // we process all events from the queue
                IGherkinProcessingTask task;
                while (tasks.TryDequeue(out task))
                {
                    // we try to merge the event with the following events
                    task = TryMergeWithNextEvents(task);

                    // perform the task
                    doTask(task);

                    // if there is a new event arrived in the meanwhile, 
                    // we stop processing the events. 
                    // Note: there is no Reset() call here, as this event 
                    // has to pass through the WaitOne() in the top of the loop
                    if (itemsAvailableEvent.WaitOne(0))
                        break;

                    cancellationToken.ThrowIfCancellationRequested();
                }

                lastProcessed = DateTime.Now;
            }
        }

        private IGherkinProcessingTask TryMergeWithNextEvents(IGherkinProcessingTask task)
        {
            IGherkinProcessingTask nextTask;
            while (tasks.TryPeek(out nextTask))
            {
                // trying to merge with the next task (=peek)
                var mergedTask = task.Merge(nextTask);
                if (mergedTask == null)
                    break;

                // if succeeded, we "eat up" the merged task and move on 
                tasks.Dequeue();
                task = mergedTask;
            }
            return task;
        }

        private bool IsFirstTask(DateTime lastProcessed)
        {
            return DateTime.Now - lastProcessed > TimeSpan.FromMilliseconds(1000);
        }

        /// <summary>
        /// Delays the processing of the other events (if any) without queuing another task
        /// </summary>
        public void Ping()
        {
            AssertRunning();

            // ping task is not a real task, it is only used for delaying the other events
            if (!tasks.IsEmpty)
                itemsAvailableEvent.Set();
        }

        public void EnqueueTask(IGherkinProcessingTask task)
        {
            AssertRunning();

            tasks.Enqueue(task);
            itemsAvailableEvent.Set();
        }

        private void AssertRunning()
        {
            if (thread == null)
                throw new InvalidOperationException("IdleQueue is not started yet or it is already disposed.");
        }

        public void Dispose()
        {
            if (thread == null)
                return;

            cancellationTokenSource.Cancel(true);
            itemsAvailableEvent.Set();
            try
            {
                thread.Join();
                thread = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "IdleQueue.Dispose");
            }
            itemsAvailableEvent.Dispose();
        }
    }

    internal static class ConcurrentQueueExtensions
    {
        public static bool Dequeue<T>(this ConcurrentQueue<T> queue)
        {
            T dummy;
            return queue.TryDequeue(out dummy);
        }
    }
}