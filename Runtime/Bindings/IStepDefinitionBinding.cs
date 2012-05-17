using System;
using System.Reflection;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepDefinitionBinding : IScopedBinding, IBinding
    {
        MethodInfo MethodInfo { get; }
        Type[] ParameterTypes { get; }
    }
}