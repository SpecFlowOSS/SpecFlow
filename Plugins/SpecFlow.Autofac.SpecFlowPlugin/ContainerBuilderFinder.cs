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
        private readonly Lazy<Func<ContainerBuilder, ContainerBuilder>> createConfigureGlobalContainer;
        private readonly Lazy<Func<ContainerBuilder, ContainerBuilder>> createConfigureScenarioContainer;
        private readonly Lazy<Func<ContainerBuilder, ContainerBuilder>> createScenarioContainerBuilder;

        public ContainerBuilderFinder(IBindingRegistry bindingRegistry, IRuntimeBindingRegistryBuilder bindingRegistryBuilder, ITestAssemblyProvider testAssemblyProvider)
        {
            this.bindingRegistry = bindingRegistry;
            this.bindingRegistryBuilder = bindingRegistryBuilder;
            this.testAssemblyProvider = testAssemblyProvider;
            static ContainerBuilder invokeVoidAndReturnBuilder(ContainerBuilder containerBuilder, MethodInfo methodInfo)
            {
                methodInfo.Invoke(null, new[] { containerBuilder });
                return containerBuilder;
            }
            createConfigureGlobalContainer = new Lazy<Func<ContainerBuilder, ContainerBuilder>>(() => FindCreateScenarioContainerBuilder(typeof(GlobalDependenciesAttribute), typeof(void), invokeVoidAndReturnBuilder), true);
            createConfigureScenarioContainer = new Lazy<Func<ContainerBuilder, ContainerBuilder>>(() => FindCreateScenarioContainerBuilder(typeof(ScenarioDependenciesAttribute), typeof(void), invokeVoidAndReturnBuilder), true);
            createScenarioContainerBuilder = new Lazy<Func<ContainerBuilder, ContainerBuilder>>(() => FindCreateScenarioContainerBuilder(typeof(ScenarioDependenciesAttribute), typeof(ContainerBuilder), (c, m) => (ContainerBuilder)m.Invoke(null, null)), true);
        }

        public Func<ContainerBuilder, ContainerBuilder> GetConfigureGlobalContainer()
        {
            bindingRegistryBuilder.BuildBindingsFromAssembly(testAssemblyProvider.TestAssembly);
            return createConfigureGlobalContainer.Value;
        }

        public Func<ContainerBuilder, ContainerBuilder> GetConfigureScenarioContainer()
        {
            return createConfigureScenarioContainer.Value;
        }

        public Func<ContainerBuilder, ContainerBuilder> GetCreateScenarioContainerBuilder()
        {
            return createScenarioContainerBuilder.Value;
        }

        protected virtual Func<ContainerBuilder, ContainerBuilder> FindCreateScenarioContainerBuilder(Type attributeType, Type returnType, Func<ContainerBuilder, MethodInfo, ContainerBuilder> invoke)
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
                            return (containerBuilder) => invoke(containerBuilder, methodInfo);
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