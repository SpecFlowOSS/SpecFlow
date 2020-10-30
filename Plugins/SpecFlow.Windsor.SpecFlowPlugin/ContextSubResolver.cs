using BoDi;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using TechTalk.SpecFlow;

namespace SpecFlow.Windsor
{
    public class ContextSubResolver<T> : ISubDependencyResolver
        where T : SpecFlowContext
    {
        private readonly IObjectContainer container;

        public ContextSubResolver(IObjectContainer container)
        {
            this.container = container;
        }

        public bool CanResolve(
            CreationContext context, 
            ISubDependencyResolver contextHandlerResolver, 
            ComponentModel model, 
            DependencyModel dependency)
        {
            return dependency.TargetType == typeof(T);
        }

        public object Resolve(
            CreationContext context, 
            ISubDependencyResolver contextHandlerResolver, 
            ComponentModel model, 
            DependencyModel dependency)
        {
            return container.Resolve<T>();
        }
    }
}
