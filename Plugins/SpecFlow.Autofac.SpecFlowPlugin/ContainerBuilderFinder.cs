using System;
using System.Linq;
using System.Reflection;
using Autofac;
using TechTalk.SpecFlow.Bindings;

namespace SpecFlow.Autofac
{
    public class ContainerBuilderFinder : IContainerBuilderFinder
    {
        private readonly IBindingRegistry bindingRegistry;
        private readonly Lazy<Func<ContainerBuilder>> createScenarioContainerBuilder;

        public ContainerBuilderFinder(IBindingRegistry bindingRegistry)
        {
            this.bindingRegistry = bindingRegistry;
            createScenarioContainerBuilder = new Lazy<Func<ContainerBuilder>>(FindCreateScenarioContainerBuilder, true);
        }

        public Func<ContainerBuilder> GetCreateScenarioContainerBuilder()
        {
            var builder = createScenarioContainerBuilder.Value;
            if (builder == null)
                throw new Exception("Unable to find scenario dependencies! Mark a static method that returns a ContainerBuilder with [ScenarioDependencies]!");
            return builder;
        }

        protected virtual Func<ContainerBuilder> FindCreateScenarioContainerBuilder()
        {
            var assemblies = bindingRegistry.GetBindingAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(m => Attribute.IsDefined((MemberInfo) m, typeof(ScenarioDependenciesAttribute))))
                    {
                        return () => (ContainerBuilder)methodInfo.Invoke(null, null);
                    }
                }
            }
            return null;

            //return (assemblies
            //    .SelectMany(assembly => assembly.GetTypes(), (_, type) => type)
            //    .SelectMany(
            //        type => type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(m => Attribute.IsDefined(m, typeof (ScenarioDependenciesAttribute))),
            //        (_, methodInfo) => (Func<ContainerBuilder>) (() => (ContainerBuilder) methodInfo.Invoke(null, null)))).FirstOrDefault();
        }
    }
}