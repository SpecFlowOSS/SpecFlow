using System;
using Autofac;
using SpecFlow.Autofac;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(AutofacPlugin))]

namespace SpecFlow.Autofac
{
    using BoDi;

    using TechTalk.SpecFlow;

    public class AutofacPlugin : IRuntimePlugin
    {
        private static Object _registrationLock = new Object();

        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
            {
                // temporary fix for CustomizeGlobalDependencies called multiple times
                // see https://github.com/techtalk/SpecFlow/issues/948
                if (!args.ObjectContainer.IsRegistered<IContainerBuilderFinder>())
                {
                    // an extra lock to ensure that there are not two super fast threads re-registering the same stuff
                    lock (_registrationLock)
                    {
                        if (!args.ObjectContainer.IsRegistered<IContainerBuilderFinder>())
                        {
                            args.ObjectContainer.RegisterTypeAs<AutofacTestObjectResolver, ITestObjectResolver>();
                            args.ObjectContainer.RegisterTypeAs<ContainerBuilderFinder, IContainerBuilderFinder>();
                        }
                    }

                    // workaround for parallel execution issue - this should be rather a feature in BoDi?
                    args.ObjectContainer.Resolve<IContainerBuilderFinder>();
                }
            };

            runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterFactoryAs<IComponentContext>(() =>
                {
                    var containerBuilderFinder = args.ObjectContainer.Resolve<IContainerBuilderFinder>();
                    var createScenarioContainerBuilder = containerBuilderFinder.GetCreateScenarioContainerBuilder();
                    var containerBuilder = createScenarioContainerBuilder();
                    RegisterSpecflowDependecies(args.ObjectContainer, containerBuilder);
                    var container = containerBuilder.Build();
                    return container.BeginLifetimeScope();
                });
            };
        }

        /// <summary>
        ///     Fix for https://github.com/gasparnagy/SpecFlow.Autofac/issues/11 Cannot resolve ScenarioInfo
        ///     Extracted from
        ///     https://github.com/techtalk/SpecFlow/blob/master/TechTalk.SpecFlow/Infrastructure/ITestObjectResolver.cs
        ///     The test objects might be dependent on particular SpecFlow infrastructure, therefore the implemented
        ///     resolution logic should support resolving the following objects (from the provided SpecFlow container):
        ///     <see cref="ScenarioContext" />, <see cref="FeatureContext" />, <see cref="TestThreadContext" /> and
        ///     <see cref="IObjectContainer" /> (to be able to resolve any other SpecFlow infrastucture). So basically
        ///     the resolution of these classes has to be forwarded to the original container.
        /// </summary>
        /// <param name="objectContainer">SpecFlow DI container.</param>
        /// <param name="containerBuilder">Autofac ContainerBuilder.</param>
        private void RegisterSpecflowDependecies(
            IObjectContainer objectContainer,
            global::Autofac.ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(ctx => objectContainer).As<IObjectContainer>();
            containerBuilder.Register(
                ctx =>
                {
                    var specflowContainer = ctx.Resolve<IObjectContainer>();
                    var scenarioContext = specflowContainer.Resolve<ScenarioContext>();
                    return scenarioContext;
                }).As<ScenarioContext>();
            containerBuilder.Register(
                ctx =>
                {
                    var specflowContainer = ctx.Resolve<IObjectContainer>();
                    var scenarioContext = specflowContainer.Resolve<FeatureContext>();
                    return scenarioContext;
                }).As<FeatureContext>();
            containerBuilder.Register(
                ctx =>
                {
                    var specflowContainer = ctx.Resolve<IObjectContainer>();
                    var scenarioContext = specflowContainer.Resolve<TestThreadContext>();
                    return scenarioContext;
                }).As<TestThreadContext>();
        }
    }
}
