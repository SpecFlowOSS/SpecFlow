using System;
using BoDi;
using Castle.Windsor;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.Windsor
{
    public class WindsorTestObjectResolver : ITestObjectResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer scenarioContainer)
        {
            var componentContext = scenarioContainer.Resolve<IWindsorContainer>();
            return componentContext.Resolve(bindingType);
        }
    }
}
