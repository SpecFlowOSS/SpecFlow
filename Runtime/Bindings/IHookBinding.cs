using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IHookBinding : IScopedBinding
    {
        void Invoke(IContextManager contextManager, ITestTracer testTracer);
    }
}