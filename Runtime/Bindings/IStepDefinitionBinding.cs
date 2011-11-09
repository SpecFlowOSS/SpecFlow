using System;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepDefinitionBinding : IScopedBinding
    {
        MethodInfo MethodInfo { get; }
        Type[] ParameterTypes { get; }

        void Invoke(IContextManager contextManager, ITestTracer testTracer, object[] arguments, out TimeSpan duration);
    }
}