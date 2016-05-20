using System;
using System.Linq;
using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class BindingInstanceResolver : IBindingInstanceResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer scenarioContainer)
        {
            return scenarioContainer.Resolve(bindingType);
        }
    }
}
