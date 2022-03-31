using System;
using System.Linq;
using System.Reflection;
using Autofac;
using SpecFlow.Autofac.SpecFlowPlugin;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Infrastructure;
using ContainerBuilder = Autofac.ContainerBuilder;

namespace SpecFlow.Autofac
{

    public class ContainerBuilderFinder : IContainerBuilderFinder
    {
        private readonly IBindingRegistry bindingRegistry;
        private readonly IRuntimeBindingRegistryBuilder bindingRegistryBuilder;
        private readonly ITestAssemblyProvider testAssemblyProvider;
        private readonly Lazy<Func<ContainerBuilder>> createScenarioContainerBuilder;
        private readonly Lazy<Func<IContainer>> createGlobalContainerBuilder;
        private readonly Lazy<Func<ContainerBuilder, Func<object>>> createScenarioContainerBuilderWithParameter;

        public ContainerBuilderFinder(IBindingRegistry bindingRegistry, IRuntimeBindingRegistryBuilder bindingRegistryBuilder, ITestAssemblyProvider testAssemblyProvider)
        {
            this.bindingRegistry = bindingRegistry;
            this.bindingRegistryBuilder = bindingRegistryBuilder;
            this.testAssemblyProvider = testAssemblyProvider;
            createGlobalContainerBuilder = new Lazy<Func<IContainer>>(() => FindCreateScenarioContainerBuilder(typeof(GlobalDependenciesAttribute), typeof(IContainer), m => (IContainer)m.Invoke(null, null)), true);
            createScenarioContainerBuilder = new Lazy<Func<ContainerBuilder>>(() => FindCreateScenarioContainerBuilder(typeof(ScenarioDependenciesAttribute), typeof(ContainerBuilder), m => (ContainerBuilder)m.Invoke(null, null)), true);
            createScenarioContainerBuilderWithParameter = new Lazy<Func<ContainerBuilder, Func<object>>>(() => c => FindCreateScenarioContainerBuilder(typeof(ScenarioDependenciesAttribute), typeof(void), m => m.Invoke(null, new[] { c })), true);
        }
        public Func<IContainer> GetCreateGlobalContainer()
        {
            bindingRegistryBuilder.BuildBindingsFromAssembly(testAssemblyProvider.TestAssembly);
            return createGlobalContainerBuilder.Value ?? null;
        }

        public Func<ContainerBuilder> GetCreateScenarioContainerBuilder()
        {
            return createScenarioContainerBuilder.Value
               ?? throw new Exception("Unable to find scenario dependencies! Mark a static method that returns a ContainerBuilder with [ScenarioDependencies]!");
        }

        public Func<ContainerBuilder, object> GetCreateScenarioContainerBuilderWithParameter()
        {
            var builder = createScenarioContainerBuilderWithParameter.Value;
            return builder != null
               ? containerBuilder => builder(containerBuilder)()
               : throw new Exception("Unable to find scenario dependencies! Mark a static method that has a ContainerBuilder parameter and returns void with [ScenarioDependencies]!");
        }

        protected virtual Func<TReturnType> FindCreateScenarioContainerBuilder<TReturnType>(Type attributeType, Type returnType, Func<MethodInfo, TReturnType> invoke)
        {
            var assemblies = bindingRegistry.GetBindingAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(m => Attribute.IsDefined(m, attributeType)))
                    {
                        if (methodInfo.ReturnType == returnType)
                        {
                            return () => invoke(methodInfo);
                        }
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