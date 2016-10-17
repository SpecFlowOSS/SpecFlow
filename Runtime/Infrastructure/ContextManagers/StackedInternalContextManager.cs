using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure.ContextManagers
{
    /// <summary>
    /// Implementation of internal context manager which keeps a stack of contexts, rather than a single one. 
    /// This allows the contexts to be used when a new context is created before the previous context has been completed 
    /// which is what happens when a step calls other steps. This means that the step contexts will be reported 
    /// correctly even when there is a nesting of steps calling steps calling steps.
    /// </summary>
    /// <typeparam name="TContext">A type derived from SpecFlowContext, which needs to be managed  in a way</typeparam>
    internal class StackedInternalContextManager<TContext> : IInternalContextManager<TContext> where TContext : SpecFlowContext
    {
        private readonly ITestTracer testTracer;
        private readonly Stack<TContext> instances = new Stack<TContext>();

        public StackedInternalContextManager(ITestTracer testTracer)
        {
            this.testTracer = testTracer;
        }

        public TContext Instance
        {
            get { return instances.Any() ? instances.Peek() : null; }
        }

        public void Init(TContext newInstance)
        {
            instances.Push(newInstance);
        }

        public void Cleanup()
        {
            if (!instances.Any())
            {
                testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                return;
            }
            var instance = instances.Pop();
            ((IDisposable)instance).Dispose();

        }

        public void Dispose()
        {
            var instance = instances.Pop();
            if (instance != null)
            {
                ((IDisposable)instance).Dispose();

            }
        }
    }
}