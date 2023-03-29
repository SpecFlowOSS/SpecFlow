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
        private readonly Lazy<Func<ILifetimeScope>> getFeatureLifetimeScope;

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
            getFeatureLifetimeScope = new Lazy<Func<ILifetimeScope>>(() => FindLifetimeScope(typeof(FeatureDependenciesAttribute), typeof(ILifetimeScope), m => (ILifetimeScope)m.Invoke(null, null)));
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

        public Func<ILifetimeScope> GetFeatureLifetimeScope()
        {
            return getFeatureLifetimeScope.Value;
        }

        protected virtual Func<ILifetimeScope> FindLifetimeScope(Type attributeType, Type returnType, Func<MethodInfo, ILifetimeScope> invoke)
        {
            var method = GetMethod(attributeType, returnType);

            return method == null
                ? null
                : () => invoke(method);
        }

        protected virtual Func<ContainerBuilder, ContainerBuilder> FindCreateScenarioContainerBuilder(Type attributeType, Type returnType, Func<ContainerBuilder, MethodInfo, ContainerBuilder> invoke)
        {
            var method = GetMethod(attributeType, returnType);

            return method == null
                ? null
                : containerBuilder => invoke(containerBuilder, method);
        }

        private MethodInfo GetMethod(Type attributeType, Type returnType)
        {
            return bindingRegistry.GetBindingAssemblies()
                                  .SelectMany(x => x.GetTypes())
                                  .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                                  .Where(x => x.ReturnType == returnType)
                                  .FirstOrDefault(x => Attribute.IsDefined(x, attributeType));
        }
    }
}