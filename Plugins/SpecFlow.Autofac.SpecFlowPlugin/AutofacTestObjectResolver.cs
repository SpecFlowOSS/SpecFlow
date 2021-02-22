using System;
using Autofac;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.Autofac
{
    public class AutofacTestObjectResolver : ITestObjectResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer scenarioContainer)
        {
            if (scenarioContainer.IsRegistered<IComponentContext>())
            {
                var componentContext = scenarioContainer.Resolve<IComponentContext>();
                return componentContext.Resolve(bindingType);
            }
            return scenarioContainer.Resolve(bindingType);
        }
    }
}
