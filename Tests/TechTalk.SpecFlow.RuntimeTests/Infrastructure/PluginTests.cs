using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BoDi;
using Moq;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    
    public class PluginTests
    {
        public class PluginWithCustomDependency : IRuntimePlugin
        {
            public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                runtimePluginEvents.RegisterGlobalDependencies += (sender, args) => args.ObjectContainer.RegisterTypeAs<CustomDependency, ICustomDependency>();
            }
        }

        public class PluginWithTraceListenerWhenStopAtFirstErrorIsTrue : IRuntimePlugin
        {
            public void RegisterDependencies(ObjectContainer container)
            {
            }

            public void RegisterCustomizations(ObjectContainer container, SpecFlow.Configuration.SpecFlowConfiguration specFlowConfiguration)
            {
                if (specFlowConfiguration.StopAtFirstError)
                    container.RegisterTypeAs<DefaultListener, ITraceListener>();
            }

            public void RegisterConfigurationDefaults(SpecFlow.Configuration.SpecFlowConfiguration specFlowConfiguration)
            {
            }

            public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
                                                                   {
                                                                       if (args.SpecFlowConfiguration.StopAtFirstError)
                                                                           args.ObjectContainer.RegisterTypeAs<DefaultListener, ITraceListener>();
                                                                   };
            }
        }

        public class PluginWithCustomConfiguration : IRuntimePlugin
        {
            private readonly Action<SpecFlow.Configuration.SpecFlowConfiguration> specifyDefaults;

            public PluginWithCustomConfiguration(Action<SpecFlow.Configuration.SpecFlowConfiguration> specifyDefaults)
            {
                this.specifyDefaults = specifyDefaults;
            }

            public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                runtimePluginEvents.ConfigurationDefaults += (sender, args) => { specifyDefaults(args.SpecFlowConfiguration); };
            }
        }

        public class PluginWithCustomTestThreadDependencies : IRuntimePlugin
        {
            private readonly Action<ObjectContainer> _specificTestRunnerDependencies;

            public PluginWithCustomTestThreadDependencies(Action<ObjectContainer> specificTestRunnerDependencies)
            {
                _specificTestRunnerDependencies = specificTestRunnerDependencies;
            }

            public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                runtimePluginEvents.CustomizeTestThreadDependencies += (sender, args) => { _specificTestRunnerDependencies(args.ObjectContainer); };
            }
        }

        public class PluginWithCustomScenarioDependencies : IRuntimePlugin
        {
            private readonly Action<ObjectContainer> _specificScenarioDependencies;

            public PluginWithCustomScenarioDependencies(Action<ObjectContainer> specificScenarioDependencies)
            {
                _specificScenarioDependencies = specificScenarioDependencies;
            }

            public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) => { _specificScenarioDependencies(args.ObjectContainer); };

            }
        }

        public class PluginWithCustomFeatureDependencies : IRuntimePlugin
        {
            private readonly Action<ObjectContainer> _specificFeatureDependencies;

            public PluginWithCustomFeatureDependencies(Action<ObjectContainer> specificFeatureDependencies)
            {
                _specificFeatureDependencies = specificFeatureDependencies;
            }

            public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                runtimePluginEvents.CustomizeFeatureDependencies += (sender, args) => { _specificFeatureDependencies(args.ObjectContainer); };
            }
        }

        public interface ICustomDependency
        {

        }

        public class CustomDependency : ICustomDependency
        {

        }

        private class CustomTestRunnerFactory : ITestRunnerFactory
        {
            public ITestRunner Create(Assembly testAssembly)
            {
                throw new NotImplementedException();
            }
        }

        public class CustomTraceListener : ITraceListener
        {
            public void WriteTestOutput(string message)
            {

            }

            public void WriteToolOutput(string message)
            {

            }
        }

        

        private StringConfigProvider GetConfigWithPlugin()
        {
            return new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <configuration>
                <specFlow>
                  
                </specFlow>
              </configuration>"));
        }

        //[Fact]
        //public void Should_be_able_to_specify_a_plugin_with_parameters()
        //{
        //    StringConfigProvider configurationHolder = new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        //      <configuration>
        //        <specFlow>
        //          <plugins>
        //            <add name=""MyCompany.MyPlugin"" parameters=""foo, bar"" />
        //          </plugins>
        //        </specFlow>
        //      </configuration>"));
        //    var pluginMock = new Mock<IRuntimePlugin>();
        //    ContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(pluginMock.Object);
        //    TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);

        //    pluginMock.Verify(p => p.Initialize(It.IsAny<RuntimePluginEvents>(), It.Is<RuntimePluginParameters>(pp => pp.Parameters == "foo, bar"), It.IsAny<UnitTestProviderConfiguration>()));
        //}

        [Fact]
        public void Should_be_able_to_register_dependencies_from_a_plugin()
        {
            var pluginWithCustomDependency = new PluginWithCustomDependency();
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomDependency.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());
            
            var container = new ObjectContainer();
            runtimePluginEvents.RaiseRegisterGlobalDependencies(container);

            var customDependency = container.Resolve<ICustomDependency>();
            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Fact]
        public void Should_be_able_to_change_default_configuration_from_a_plugin()
        {
            var pluginWithCustomConfiguration = new PluginWithCustomConfiguration(conf => conf.StopAtFirstError = true);
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomConfiguration.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());

            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            runtimePluginEvents.RaiseConfigurationDefaults(specFlowConfiguration);

            specFlowConfiguration.StopAtFirstError.Should().BeTrue();
        }

        [Fact]
        public void Should_be_able_to_register_further_dependencies_based_on_the_configuration()
        {
            var pluginWithCustomTestRunnerFactoryWhenStopAtFirstErrorIsTrue = new PluginWithTraceListenerWhenStopAtFirstErrorIsTrue();
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomTestRunnerFactoryWhenStopAtFirstErrorIsTrue.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());

            var container = new ObjectContainer();
            var specFlowConfiguration = ConfigurationLoader.GetDefault();
            runtimePluginEvents.RaiseCustomizeGlobalDependencies(container, specFlowConfiguration);
            container.RegisterTypeAs<NullListener, ITraceListener>();
            
            specFlowConfiguration.StopAtFirstError = true;
            runtimePluginEvents.RaiseCustomizeGlobalDependencies(container, specFlowConfiguration);

            var customTestRunnerFactory = container.Resolve<ITraceListener>();
            customTestRunnerFactory.Should().BeOfType<DefaultListener>();
        }

        [Fact]
        public void Should_be_able_to_register_test_runner_dependencies_from_a_plugin()
        {
            var pluginWithCustomTestThreadDependencies = new PluginWithCustomTestThreadDependencies(oc => oc.RegisterTypeAs<CustomDependency, ICustomDependency>());
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomTestThreadDependencies.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());

            var container = new ObjectContainer();
            runtimePluginEvents.RaiseCustomizeTestThreadDependencies(container);

            var customDependency = container.Resolve<ICustomDependency>();
            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Fact]
        public void Test_runner_dependencies_from_a_plugin_are_not_in_the_global_container()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            var pluginWithCustomTestThreadDependencies = new PluginWithCustomTestThreadDependencies(oc => oc.RegisterTypeAs<CustomDependency, ICustomDependency>());
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomTestThreadDependencies.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());

            var container = new ObjectContainer();
            runtimePluginEvents.RaiseRegisterGlobalDependencies(container);

            Action resolveAction = () => container.Resolve<ICustomDependency>();

            resolveAction.Should().Throw<ObjectContainerException>();            
        }

        [Fact]
        public void Should_be_able_to_override_test_runner_registration_from_a_plugin()
        {
            var pluginWithCustomTestThreadDependencies = new PluginWithCustomTestThreadDependencies(oc => oc.RegisterTypeAs<CustomTraceListener, ITraceListener>());
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomTestThreadDependencies.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());

            var container = new ObjectContainer();
            runtimePluginEvents.RaiseCustomizeTestThreadDependencies(container);
            var traceListener = container.Resolve<ITraceListener>();

            traceListener.Should().BeOfType(typeof(CustomTraceListener));
        }

        [Fact]
        public void Should_be_able_to_register_scenario_dependencies_from_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            var pluginWithCustomScenarioDependencies = new PluginWithCustomScenarioDependencies(oc => oc.RegisterTypeAs<CustomDependency, ICustomDependency>());
            var runtimePluginEvents = new RuntimePluginEvents();
            pluginWithCustomScenarioDependencies.Initialize(runtimePluginEvents, new RuntimePluginParameters(), new UnitTestProviderConfiguration());

            var container = new ObjectContainer();
            runtimePluginEvents.RaiseCustomizeScenarioDependencies(container);

            var customDependency = container.Resolve<ICustomDependency>();

            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Fact]
        public void Should_be_able_to_register_feature_dependencies_from_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            var testDefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomFeatureDependencies(oc => oc.RegisterTypeAs<CustomDependency, ICustomDependency>()));
            var container = TestObjectFactories.CreateDefaultFeatureContainer(configurationHolder, testDefaultDependencyProvider);
            var customDependency = container.Resolve<ICustomDependency>();

            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

    }

    class TestDefaultDependencyProvider : DefaultDependencyProvider
    {
        private readonly IRuntimePlugin _pluginToReturn;

        public TestDefaultDependencyProvider(IRuntimePlugin pluginToReturn)
        {
            _pluginToReturn = pluginToReturn;
        }

        public override void RegisterGlobalContainerDefaults(BoDi.ObjectContainer container)
        {
            base.RegisterGlobalContainerDefaults(container);

            var runtimePluginLocator = new Mock<IRuntimePluginLocator>();
            runtimePluginLocator.Setup(m => m.GetAllRuntimePlugins()).Returns(new List<string>() { "aPlugin" });


            var pluginLoaderStub = new Mock<IRuntimePluginLoader>();
            var traceListener = container.Resolve<ITraceListener>();
            pluginLoaderStub.Setup(pl => pl.LoadPlugin(It.IsAny<string>(), traceListener)).Returns(_pluginToReturn);


            container.RegisterInstanceAs<IRuntimePluginLocator>(runtimePluginLocator.Object);
            container.RegisterInstanceAs<IRuntimePluginLoader>(pluginLoaderStub.Object);

        }
    }

}
