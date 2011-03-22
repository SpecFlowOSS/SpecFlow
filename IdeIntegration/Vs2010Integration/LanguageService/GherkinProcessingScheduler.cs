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

    public class DelegateTask : IGherkinProcessingTask
    {
        private readonly Action task;

        public DelegateTask(Action task)
        {
            this.task = task;
        }

        public void Apply()
        {
            try
            {
                task();
            }
            catch(Exception)
            {
                //TODO: log error
            }
        }

        public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
        {
            return null;
        }
    }

    internal class PingTask : IGherkinProcessingTask
    {
        public static readonly IGherkinProcessingTask Instance = new PingTask();

        public void Apply()
        {
            //nop;
        }

        public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
        {
            return other;
        }
    }


    public class GherkinProcessingScheduler : IDisposable
    {
        private readonly IVisualStudioTracer visualStudioTracer;
        private static readonly TimeSpan parsingDelay = TimeSpan.FromMilliseconds(250);
        private static readonly TimeSpan analyzingDelay = TimeSpan.FromMilliseconds(500);

        private readonly Dispatcher parsingDispatcher;
        private readonly IScheduler parsingScheduler;
        private readonly Dispatcher analyzingDispatcher;
        private readonly IScheduler analyzingScheduler;
        private readonly Subject<IGherkinProcessingTask> parsingSubject; 
        private readonly Subject<IGherkinProcessingTask> analyzingSubject; 

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

        public GherkinProcessingScheduler(IVisualStudioTracer visualStudioTracer, bool enableAnalysis)
        {
            this.visualStudioTracer = visualStudioTracer;
            
            parsingDispatcher = CreateBackgroundThreadWithDispatcher(ThreadPriority.BelowNormal);
            parsingScheduler = new DispatcherScheduler(parsingDispatcher);
            parsingSubject = new Subject<IGherkinProcessingTask>(parsingScheduler);
            parsingSubject.BufferWithTimeout(parsingDelay, parsingScheduler, flushFirst: true)
                .Subscribe(ApplyTask);

            if (enableAnalysis)
            {
                analyzingDispatcher = CreateBackgroundThreadWithDispatcher(ThreadPriority.BelowNormal);
                analyzingScheduler = new DispatcherScheduler(analyzingDispatcher);
                analyzingSubject = new Subject<IGherkinProcessingTask>(analyzingScheduler);
                analyzingSubject.BufferWithTimeout(analyzingDelay, analyzingScheduler, flushFirst: false)
                    .Subscribe(ApplyTask);
            }
        }

        private void ApplyTask(IGherkinProcessingTask task)
        {
            if (!(task is PingTask))
                visualStudioTracer.Trace("Applying task on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            task.Apply();
        }

        private void ApplyTask(IEnumerable<IGherkinProcessingTask> tasks)
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
                        if (!(currentTask is PingTask || task is PingTask))
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
            if (analyzingSubject != null)
                analyzingSubject.OnNext(PingTask.Instance);
        }

        public void EnqueueAnalyzingRequest(IGherkinProcessingTask task)
        {
            if (analyzingSubject == null)
                return;

            visualStudioTracer.Trace("Analyzing request queued on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            analyzingSubject.OnNext(task);
        }

        public void Dispose()
        {
            parsingDispatcher.InvokeShutdown();
            if (analyzingDispatcher != null)
                analyzingDispatcher.InvokeShutdown();
        }
    }
}