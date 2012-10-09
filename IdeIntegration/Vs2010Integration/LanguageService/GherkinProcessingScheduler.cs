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

            parserQueue = new IdleTaskProcessingQueue(parsingDelay, true, tracer);
            parserQueue.Start();

            if (enableAnalysis)
            {
                analyzerQueue = new IdleTaskProcessingQueue(analyzingDelay, false, tracer);
                analyzerQueue.Start();
            }
        }

        public void EnqueueParsingRequest(IGherkinProcessingTask change)
        {
            tracer.Trace("Change queued on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            if (parserQueue == null)
            {
                tracer.Trace("Parser queue is not initialized!", this);
                return;
            }

            parserQueue.EnqueueTask(change);
            if (analyzerQueue != null)
                analyzerQueue.Ping();
        }

        public void EnqueueAnalyzingRequest(IGherkinProcessingTask task)
        {
            tracer.Trace("Analyzing request queued on thread: " + Thread.CurrentThread.ManagedThreadId, "GherkinProcessingScheduler");
            if (analyzerQueue == null)
            {
                tracer.Trace("Analyzer queue is not initialized!", this);
                return;
            }

            analyzerQueue.EnqueueTask(task);
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