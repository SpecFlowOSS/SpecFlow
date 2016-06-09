using System;
using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IBindingInstanceResolver
    {
        object ResolveBindingInstance(Type bindingType, IObjectContainer scenarioContainer);
    }
}