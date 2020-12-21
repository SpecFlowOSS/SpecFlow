using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingInvoker
    {
        Task<(object result, TimeSpan duration)> InvokeBindingAsync(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer);
    }
}