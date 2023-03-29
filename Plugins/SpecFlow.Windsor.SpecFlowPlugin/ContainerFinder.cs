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
            var builder = createScenarioContainer.Value;

            if (builder == null)
                throw new Exception("Unable to find scenario dependencies! Mark a static method that returns a IWindsorContainer with [ScenarioDependencies]!");

            return builder;
        }

        protected virtual Func<IWindsorContainer> FindCreateScenarioContainer()
        {
            var method = GetCreationMethod();

            if (method == null) 
                return null;

            return () => CreateAndConfigureContainer(method);
        }

        private IWindsorContainer CreateAndConfigureContainer(MethodInfo method)
        {
            var container = CreateContainer(method);

            if (ShouldRegisterBindings(method))
                RegisterBindings(container);

            return container;
        }

        protected virtual MethodInfo GetCreationMethod()
        {
            return bindingRegistry
                   .GetBindingAssemblies()
                   .SelectMany(x => x.GetTypes())
                   .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                   .FirstOrDefault(x => Attribute.IsDefined(x, typeof(ScenarioDependenciesAttribute)));
        }

        protected virtual IWindsorContainer CreateContainer(MethodInfo method)
        {
            if (method == null) 
                return null;

            if (!(method.Invoke(null, null) is IWindsorContainer container)) 
                return null;

            return container;
        }

        private bool ShouldRegisterBindings(MethodInfo method)
        {
            var attribute = GetDependenciesAttribute(method);

            return attribute.AutoRegisterBindings;
        }

        protected virtual ScenarioDependenciesAttribute GetDependenciesAttribute(MethodInfo method)
        {
            return (ScenarioDependenciesAttribute)Attribute.GetCustomAttribute(method, typeof(ScenarioDependenciesAttribute));
        }

        protected virtual void RegisterBindings(IWindsorContainer container)
        {
            foreach (var assembly in bindingRegistry.GetBindingAssemblies())
            {
                container.Register(
                    Types.FromAssembly(assembly)
                         .Where(x => x.IsDefined(typeof(BindingAttribute), false)));
            }
        }
    }
}
