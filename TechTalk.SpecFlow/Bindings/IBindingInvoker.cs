using System;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    [Obsolete("Use async version of the interface (IAsyncBindingInvoker) whenever you can")]
    public interface IBindingInvoker
    {
        [Obsolete("Use async version of the method of IAsyncBindingInvoker instead")]
        object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration);
    }
}