using System.Reflection;
using BoDi;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TechTalk.SpecFlow;

namespace SpecFlow.Windsor
{
    public static class WindsorContainerExtensions
    {
        public static IWindsorContainer RegisterBindings(this IWindsorContainer container, Assembly assembly)
        {
            return container.Register(
                Types.FromAssembly(assembly)
                     .Where(x => x.IsDefined(typeof(BindingAttribute), false))
                     .LifestyleScoped());
        }

        public static IWindsorContainer RegisterContexts(this IWindsorContainer container)
        {
            return container.Register(
                Component.For<ScenarioContext>().UsingFactoryMethod(kernel => kernel.Resolve<IObjectContainer>().Resolve<ScenarioContext>()),
                Component.For<FeatureContext>().UsingFactoryMethod(kernel => kernel.Resolve<IObjectContainer>().Resolve<FeatureContext>()),
                Component.For<TestThreadContext>().UsingFactoryMethod(kernel => kernel.Resolve<IObjectContainer>().Resolve<TestThreadContext>()));
        }
    }
}
