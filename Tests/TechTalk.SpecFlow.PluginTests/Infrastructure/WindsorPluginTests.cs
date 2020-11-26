using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using BoDi;
using Castle.Windsor;
using FluentAssertions;
using Moq;
using SpecFlow.Windsor;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using Xunit;

namespace TechTalk.SpecFlow.PluginTests.Infrastructure
{
    public class WindsorPluginTests
    {
        [Fact]
        public void Can_load_Windsor_plugin()
        {
            var loader = new RuntimePluginLoader();
            var listener = new Mock<ITraceListener>();

            var plugin = loader.LoadPlugin("SpecFlow.Windsor.SpecFlowPlugin.dll", listener.Object);

            plugin.Should().NotBeNull();
        }

        [Fact]
        public void Resolutions_are_forwarded_to_Windsor()
        {
            var resolver = new WindsorTestObjectResolver();

            var objectContainer = new Mock<IObjectContainer>();
            var container = new Mock<IWindsorContainer>();
            objectContainer.Setup(x => x.Resolve<IWindsorContainer>()).Returns(container.Object);

            resolver.ResolveBindingInstance(typeof(ITraceListener), objectContainer.Object);

            container.Verify(x => x.Resolve(typeof(ITraceListener)), Times.Once);
        }

        [Fact]
        public void Can_resolve_Windsor_container_from_finder()
        {
            var globalContainer = new ObjectContainer();
            var scenarioContainer = new ObjectContainer(globalContainer);

            var container = CreateContainerViaPlugin(globalContainer, scenarioContainer);

            container.Should().NotBeNull();
        }

        [Fact]
        public void Resolved_scenario_context_is_the_same_for_the_same_scenario_container()
        {
            var globalContainer = new ObjectContainer();
            var scenarioContainer = new ObjectContainer(globalContainer);

            scenarioContainer.RegisterTypeAs<WindsorTestObjectResolver, ITestObjectResolver>();
            scenarioContainer.RegisterInstanceAs(new ScenarioInfo("", "", Array.Empty<string>(), new OrderedDictionary()));

            var container = CreateContainerViaPlugin(globalContainer, scenarioContainer);

            var context1 = container.Resolve<ScenarioContext>();
            var context2 = container.Resolve<ScenarioContext>();

            context2.Should().BeSameAs(context1);
        }

        [Fact]
        public void Resolution_for_contexts_are_passed_to_object_container()
        {
            var container = new WindsorContainer();
            var objectContainer = new Mock<IObjectContainer>();
            var context = new ScenarioContext(
                objectContainer.Object, 
                new ScenarioInfo("", "", Array.Empty<string>(), new OrderedDictionary()), 
                new WindsorTestObjectResolver());

            objectContainer.Setup(x => x.Resolve<ScenarioContext>()).Returns(context);

            var plugin = new WindsorPlugin();
            plugin.RegisterSpecflowDependecies(objectContainer.Object, container);

            container.Resolve<ScenarioContext>();

            objectContainer.Verify(x => x.Resolve<ScenarioContext>(), Times.Once);
        }

        [Fact]
        public void Feature_contexts_persist_across_scenarios()
        {
            var plugin = new WindsorPlugin();
            var events = new RuntimePluginEvents();

            var globalContainer = new ObjectContainer();
            var featureContainer = new ObjectContainer(globalContainer);
            var scenarioContainer1 = new ObjectContainer(featureContainer);
            var scenarioContainer2 = new ObjectContainer(featureContainer);

            var context = new FeatureContext(featureContainer, new FeatureInfo(CultureInfo.CurrentCulture, "", "", ""), ConfigurationLoader.GetDefault());

            globalContainer.RegisterInstanceAs<IContainerFinder>(new SpecificContainerFinder(() => new WindsorContainer()));
            featureContainer.RegisterInstanceAs(context);

            plugin.Initialize(events, null, null);
            events.RaiseCustomizeGlobalDependencies(globalContainer, null);
            events.RaiseCustomizeFeatureDependencies(featureContainer);
            events.RaiseCustomizeScenarioDependencies(scenarioContainer1);
            var context1 = scenarioContainer1.Resolve<IWindsorContainer>().Resolve<FeatureContext>();

            events.RaiseCustomizeScenarioDependencies(scenarioContainer2);
            var context2 = scenarioContainer2.Resolve<IWindsorContainer>().Resolve<FeatureContext>();

            context2.Should().BeSameAs(context1);
        }

        [Fact]
        public void Bindings_registered_by_default()
        {
            var registry = new Mock<IBindingRegistry>();
            var finder = new BindingRegistryContainerFinder(registry.Object, new ScenarioDependenciesAttribute());

            finder.GetCreateScenarioContainer()();

            finder.RegisterBindingsCalled.Should().BeTrue();
        }

        [Fact]
        public void Bindings_not_registered_when_specified()
        {
            var registry = new Mock<IBindingRegistry>();
            var finder = new BindingRegistryContainerFinder(registry.Object, new ScenarioDependenciesAttribute { AutoRegisterBindings = false });

            finder.GetCreateScenarioContainer()();

            finder.RegisterBindingsCalled.Should().BeFalse();
        }

        private IWindsorContainer CreateContainerViaPlugin(ObjectContainer globalContainer, ObjectContainer scenarioContainer)
        {
            var plugin = new WindsorPlugin();
            var events = new RuntimePluginEvents();
            
            globalContainer.RegisterInstanceAs<IContainerFinder>(new SpecificContainerFinder(() => new WindsorContainer()));

            plugin.Initialize(events, null, null);
            events.RaiseCustomizeGlobalDependencies(globalContainer, null);
            events.RaiseCustomizeScenarioDependencies(scenarioContainer);

            return scenarioContainer.Resolve<IWindsorContainer>();
        }

        private class SpecificContainerFinder : ContainerFinder
        {
            private readonly Func<IWindsorContainer> containerBuilder;

            public SpecificContainerFinder(Func<IWindsorContainer> containerBuilder)
                : base(null)
            {
                this.containerBuilder = containerBuilder;
            }

            protected override Func<IWindsorContainer> FindCreateScenarioContainer()
            {
                return containerBuilder;
            }
        }

        private class BindingRegistryContainerFinder : ContainerFinder
        {
            private readonly ScenarioDependenciesAttribute attribute;

            public BindingRegistryContainerFinder(IBindingRegistry bindingRegistry, ScenarioDependenciesAttribute attribute)
                : base(bindingRegistry)
            {
                this.attribute = attribute;
            }

            public bool RegisterBindingsCalled { get; set; }

            protected override MethodInfo GetCreationMethod()
            {
                return new Mock<MethodInfo>().Object;
            }

            protected override IWindsorContainer CreateContainer(MethodInfo method)
            {
                return null;
            }

            protected override ScenarioDependenciesAttribute GetDependenciesAttribute(MethodInfo method)
            {
                return attribute;
            }

            protected override void RegisterBindings(IWindsorContainer container)
            {
                RegisterBindingsCalled = true;
            }
        }
    }
}
