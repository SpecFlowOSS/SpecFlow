using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace SpecFlow.Windsor
{
    public class ContainerFinder : IContainerFinder
    {
        private readonly IBindingRegistry bindingRegistry;
        private readonly Lazy<Func<IWindsorContainer>> createScenarioContainer;

        public ContainerFinder(IBindingRegistry bindingRegistry)
        {
            this.bindingRegistry = bindingRegistry;

            createScenarioContainer = new Lazy<Func<IWindsorContainer>>(FindCreateScenarioContainer, true);
        }

        public Func<IWindsorContainer> GetCreateScenarioContainer()
        {
            var scenarioBuilder = createScenarioContainer.Value;

            if (scenarioBuilder == null)
                throw new Exception("Unable to find dependencies. Mark a static method that returns a IWindsorContainer with [ScenarioDependencies]");

            return scenarioBuilder;
        }
        
        protected virtual Func<IWindsorContainer> FindCreateScenarioContainer()
        {
            var descriptor = GetMethodDescriptor<ScenarioDependenciesAttribute>();

            if (descriptor == null) 
                return null;

            return () => GetContainer(descriptor.Method, descriptor.Attribute.AutoRegister);
        }

        private MethodDescriptor<T> GetMethodDescriptor<T>()
            where T : Attribute
        {
            return bindingRegistry.GetBindingAssemblies()
                                  .SelectMany(x => x.GetTypes())
                                  .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                                  .Where(x => Attribute.IsDefined(x, typeof(T)))
                                  .Select(x => new MethodDescriptor<T>(x, (T)Attribute.GetCustomAttribute(x, typeof(T))))
                                  .FirstOrDefault();
        }

        private IWindsorContainer GetContainer(MethodInfo method, SpecFlowDependencies dependencies)
        {
            if (!(method.Invoke(null, null) is IWindsorContainer container)) 
                return null;

            if (dependencies.HasFlag(SpecFlowDependencies.Bindings))
            {
                foreach (var assembly in bindingRegistry.GetBindingAssemblies())
                {
                    container.Register(
                        Types.FromAssembly(assembly)
                             .Where(x => x.IsDefined(typeof(BindingAttribute), false))
                             .LifestyleScoped());
                }
            }

            if (dependencies.HasFlag(SpecFlowDependencies.Contexts))
            {
                container.Register(
                    Component.For<ScenarioContext>().LifestyleScoped(),
                    Component.For<FeatureContext>().LifestyleScoped(),
                    Component.For<TestThreadContext>().LifestyleScoped());
            }

            return container;
        }

        private class MethodDescriptor<T>
            where T : Attribute
        {
            public MethodDescriptor(MethodInfo method, T attribute)
            {
                Method = method;
                Attribute = attribute;
            }

            public MethodInfo Method { get; }

            public T Attribute { get; }
        }
    }
}
