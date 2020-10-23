using System;
using BoDi;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using SpecFlow.Windsor;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(WindsorPlugin))]

namespace SpecFlow.Windsor
{
    public class WindsorPlugin : IRuntimePlugin
    {
        private static Object _registrationLock = new Object();

        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
            {
                // temporary fix for CustomizeGlobalDependencies called multiple times
                // see https://github.com/techtalk/SpecFlow/issues/948
                if (!args.ObjectContainer.IsRegistered<IContainerFinder>())
                {
                    // an extra lock to ensure that there are not two super fast threads re-registering the same stuff
                    lock (_registrationLock)
                    {
                        if (!args.ObjectContainer.IsRegistered<IContainerFinder>())
                        {
                            args.ObjectContainer.RegisterTypeAs<WindsorTestObjectResolver, ITestObjectResolver>();
                            args.ObjectContainer.RegisterTypeAs<ContainerFinder, IContainerFinder>();
                        }
                    }

                    // workaround for parallel execution issue - this should be rather a feature in BoDi?
                    args.ObjectContainer.Resolve<IContainerFinder>();
                }
            };

            runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterFactoryAs(() =>
                {
                    var containerBuilderFinder = args.ObjectContainer.Resolve<IContainerFinder>();
                    var createScenarioContainer = containerBuilderFinder.GetCreateScenarioContainer();
                    var container = createScenarioContainer();

                    RegisterSpecflowDependecies(args.ObjectContainer, container);

                    return container;
                });
            };
        }

        private void RegisterSpecflowDependecies(
            IObjectContainer objectContainer,
            IWindsorContainer container)
        {
            container.Register(
                Component.For<IObjectContainer>().Instance(objectContainer),
                Component.For<ScenarioContext>().Instance(objectContainer.Resolve<ScenarioContext>()),
                Component.For<FeatureContext>().Instance(objectContainer.Resolve<FeatureContext>()),
                Component.For<TestThreadContext>().Instance(objectContainer.Resolve<TestThreadContext>()));
        }
    }
}
