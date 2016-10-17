using System;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure.ContextManagers
{
    internal class InternalContextManager<TContext> : IInternalContextManager<TContext> where TContext : SpecFlowContext
    {
        private readonly ITestTracer testTracer;
        private TContext instance;

        public InternalContextManager(ITestTracer testTracer)
        {
            this.testTracer = testTracer;
        }

        public TContext Instance
        {
            get { return instance; }
        }

        public void Init(TContext newInstance)
        {
            if (instance != null)
            {
                testTracer.TraceWarning(string.Format("The previous {0} was not disposed.", typeof(TContext).Name));
                Dispose();
            }
            instance = newInstance;
        }

        public void Cleanup()
        {
            if (instance == null)
            {
                testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                return;
            }
            ((IDisposable)instance).Dispose();
            instance = null;
        }

        public void Dispose()
        {
            if (instance != null)
            {
                ((IDisposable)instance).Dispose();
                instance = null;
            }
        }
    }
}