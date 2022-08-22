using System;
using Autofac;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.Autofac
{
    public class AutofacTestObjectResolver : ITestObjectResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer container)
        {
            if (container.IsRegistered<IComponentContext>())
            {
                var componentContext = container.Resolve<IComponentContext>();
                return componentContext.Resolve(bindingType);
            }

            if (container.IsRegistered<ILifetimeScope>())
            {
                var lifeTimeScope = container.Resolve<ILifetimeScope>();
                return lifeTimeScope.Resolve(bindingType);
            }

            return container.Resolve(bindingType);
        }
    }
}
