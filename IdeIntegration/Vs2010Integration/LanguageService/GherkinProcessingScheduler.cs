using System;
using System.Linq;
using System.Collections.Generic;
using System.Concurrency;
using System.Threading;
using System.Windows.Threading;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinProcessingTask
    {
        void Apply();
        IGherkinProcessingTask Merge(IGherkinProcessingTask other);
    }

    public class GherkinProcessingScheduler : IDisposable
    {
        private readonly IVisualStudioTracer visualStudioTracer;
        private static readonly TimeSpan parsingDelay = TimeSpan.FromMilliseconds(250);

        private readonly Dispatcher parsingDispatcher;
        private readonly IScheduler parsingScheduler;
        private readonly Subject<IGherkinProcessingTask> parsingSubject; 

        private Dispatcher CreateBackgroundThreadWithDispatcher(ThreadPriority priority)
        {
            var thread = new Thread(Dispatcher.Run)
                        {
                            IsBackground = true, Priority = priority
                        };
            thread.Start();

            Dispatcher result;
            while ((result = Dispatcher.FromThread(thread)) == null)
            {
                Thread.Sleep(10);
            }

            return result;
        }

        public GherkinProcessingScheduler(IVisualStudioTracer visualStudioTracer)
        {
            this.visualStudioTracer = visualStudioTracer;
            
            parsingDispatcher = CreateBackgroundThreadWithDispatcher(ThreadPriority.BelowNormal);
            parsingScheduler = new DispatcherScheduler(parsingDispatcher);
            parsingSubject = new Subject<IGherkinProcessingTask>(parsingScheduler);
            parsingSubject.BufferWithTimeout(parsingDelay, parsingScheduler, flushFirst: true)
                .Subscribe(ApplyTask);
        }

        private void ApplyTask(IGherkinProcessingTask task)
        {
            visualStudioTracer.Trace("Applying task on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            task.Apply();
        }

        private void ApplyTask(IGherkinProcessingTask[] tasks)
        {
            IGherkinProcessingTask currentTask = null;
            foreach (var task in tasks)
            {
                if (currentTask == null)
                    currentTask = task;
                else
                {
                    var mergedTask = currentTask.Merge(task);
                    if (mergedTask == null) // cannot merge
                    {
                        ApplyTask(currentTask);
                        currentTask = task;
                    }
                    else
                    {
                        visualStudioTracer.Trace("Task merged", "GherkinProcessingScheduler");
                        currentTask = mergedTask;
                    }
                }
            }
            if (currentTask != null)
                ApplyTask(currentTask);
        }

        public void EnqueueParsingRequest(IGherkinProcessingTask change)
        {
            visualStudioTracer.Trace("Change queued on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            parsingSubject.OnNext(change);
        }

        public void Dispose()
        {
            parsingDispatcher.InvokeShutdown();
        }
    }
}