using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings;

public interface IAsyncBindingInvoker
{
    Task<object> InvokeBindingAsync(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, DurationHolder durationHolder);
}
