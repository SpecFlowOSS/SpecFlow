using System;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public interface IGherkinProcessingScheduler : IDisposable
    {
        void EnqueueParsingRequest(IGherkinProcessingTask change);
        void EnqueueAnalyzingRequest(IGherkinProcessingTask task);
    }
}