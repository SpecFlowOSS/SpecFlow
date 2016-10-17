using System;

namespace TechTalk.SpecFlow.Infrastructure.ContextManagers
{
    public interface IInternalContextManager<TContext> : IDisposable
            where TContext : SpecFlowContext
    {
        TContext Instance { get; }
        void Init(TContext newInstance);
        void Cleanup();
    }
}