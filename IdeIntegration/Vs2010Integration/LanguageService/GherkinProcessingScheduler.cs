using System;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class GherkinProcessingScheduler : IDisposable
    {
        private static readonly TimeSpan parsingDelay = TimeSpan.FromMilliseconds(250);
        private static readonly TimeSpan analyzingDelay = TimeSpan.FromMilliseconds(500);

        private readonly IIdeTracer tracer;
        private IdleTaskProcessingQueue parserQueue;
        private IdleTaskProcessingQueue analyzerQueue;

        public GherkinProcessingScheduler(IIdeTracer tracer, bool enableAnalysis)
        {
            this.tracer = tracer;

            parserQueue = new IdleTaskProcessingQueue(parsingDelay, true, tracer, DoTask);
            parserQueue.Start();

            if (enableAnalysis)
            {
                analyzerQueue = new IdleTaskProcessingQueue(analyzingDelay, false, tracer, DoTask);
                analyzerQueue.Start();
            }
        }

        public void EnqueueParsingRequest(IGherkinProcessingTask change)
        {
            //tracer.Trace("Change queued on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            if (parserQueue == null)
            {
                tracer.Trace("Unable to perform parsing request: Parser queue is not initialized!", this);
                return;
            }

            parserQueue.EnqueueTask(change);
            if (analyzerQueue != null)
                analyzerQueue.Ping();
        }

        public void EnqueueAnalyzingRequest(IGherkinProcessingTask task)
        {
            tracer.Trace("Analyzing request '{1}' queued on thread: {0}", this, Thread.CurrentThread.ManagedThreadId, task);
            if (analyzerQueue == null)
            {
                tracer.Trace("Unable to perform analyzing request: Analyzer queue is not initialized!", this);
                return;
            }

            analyzerQueue.EnqueueTask(task);
        }

        private void DoTask(IGherkinProcessingTask task)
        {
            tracer.Trace("Applying task '{1}' on thread: {0}", this, Thread.CurrentThread.ManagedThreadId, task);
            try
            {
                task.Apply();
            }
            catch (Exception exception)
            {
                tracer.Trace("Task error: {0}", this, exception);
            }
        }

        public void Dispose()
        {
            if (analyzerQueue != null)
            {
                var queue = analyzerQueue;
                analyzerQueue = null; // we set the field to null first, to avoid scheduling during dispose
                queue.Dispose();
            }

            if (parserQueue != null)
            {
                var queue = parserQueue;
                parserQueue = null; // we set the field to null first, to avoid scheduling during dispose
                queue.Dispose();
            }
        }
    }
}