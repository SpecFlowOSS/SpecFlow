using System;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

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
        private readonly IIdeTracer tracer;

        public DelegateTask(Action task, IIdeTracer tracer)
        {
            this.task = task;
            this.tracer = tracer;
        }

        public void Apply()
        {
            try
            {
                task();
            }
            catch(Exception exception)
            {
                tracer.Trace("Exception: " + exception, "DelegateTask");
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
}