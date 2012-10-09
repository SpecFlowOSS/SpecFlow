using System;

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
            task();
        }

        public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
        {
            return null;
        }
    }
}