using System;
using System.Linq;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingInvoker
    {
        object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration);
    }
}