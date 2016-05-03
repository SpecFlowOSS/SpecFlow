using System;
using System.Collections.Concurrent;
using System.Threading;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure.ContextManagers
{
    internal class ThreadSafeInternalContextManagerWrapper<TContext> : IInternalContextManager<TContext>
            where TContext : SpecFlowContext
    {
        private readonly ITestTracer _testTracer;
        private readonly ConcurrentDictionary<int, IInternalContextManager<TContext>> _wrappedInstancesTracker;
        private readonly Func<ITestTracer, IInternalContextManager<TContext>> _contextManagerValueFactory;

        public ThreadSafeInternalContextManagerWrapper(ITestTracer testTracer, Func<ITestTracer, IInternalContextManager<TContext>> contentManagerValueFactory)
        {
            _testTracer = testTracer;
            _contextManagerValueFactory = contentManagerValueFactory;
            _wrappedInstancesTracker = new ConcurrentDictionary<int, IInternalContextManager<TContext>>();
        }

        public TContext Instance
        {
            get { return InternalContextManagerInstance.Instance; }
        }

        private IInternalContextManager<TContext> InternalContextManagerInstance
        {
            get
            {
                return _wrappedInstancesTracker.GetOrAdd(Thread.CurrentThread.ManagedThreadId, _contextManagerValueFactory(_testTracer));
            }
        } 

        public void Init(TContext newInstance)
        {
            InternalContextManagerInstance.Init(newInstance);
        }

        public void Cleanup()
        {
            InternalContextManagerInstance.Cleanup();
        }

        public void Dispose()
        {
            foreach (var instance in _wrappedInstancesTracker)
            {
                instance.Value.Dispose();
            }
        }
    }
}