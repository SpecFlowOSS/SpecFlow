using System;

namespace TechTalk.SpecFlow.Async
{
    // Implemented by the generated test code, to provide an easy way to
    // call into the enqueue methods. These map directly to the methods on
    // Microsoft.Silverlight.Testing.WorkItemTest
    public interface ISilverlightTestInstance
    {
        void EnqueueCallback(Action callback);
        void EnqueueCallback(Action[] callbacks);
        void EnqueueConditional(Func<bool> continueUntil);
        void EnqueueDelay(TimeSpan delay);
        void EnqueueTestComplete();
    }
}