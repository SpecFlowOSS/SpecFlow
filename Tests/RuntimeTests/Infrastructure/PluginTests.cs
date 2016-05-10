﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BoDi;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class PluginTests
    {
        public class PluginWithCustomDependency : IRuntimePlugin
        {
            public void RegisterDependencies(ObjectContainer container)
            {
                container.RegisterTypeAs<CustomDependency, ICustomDependency>();
            }

            public void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
            {
                
            }

            public void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
            {
            }

            public void CustomizeTestRunnerDependencies(ObjectContainer testRunnerContainer)
            {
                
            }
        }

        public class PluginWithCustomTestRunnerFactoryWhenStopAtFirstErrorIsTrue : IRuntimePlugin
        {
            public void RegisterDependencies(ObjectContainer container)
            {
            }

            public void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
            {
                if (runtimeConfiguration.StopAtFirstError)
                    container.RegisterTypeAs<CustomTestRunnerFactory, ITestRunnerFactory>();
            }

            public void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
            {
            }

            public void CustomizeTestRunnerDependencies(ObjectContainer testRunnerContainer)
            {
                
            }
        }

        public class PluginWithCustomConfiguration : IRuntimePlugin
        {
            private readonly Action<RuntimeConfiguration> specifyDefaults;

            public PluginWithCustomConfiguration(Action<RuntimeConfiguration> specifyDefaults)
            {
                this.specifyDefaults = specifyDefaults;
            }

            public void RegisterDependencies(ObjectContainer container)
            {
            }

            public void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
            {
            }

            public void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
            {
                specifyDefaults(runtimeConfiguration);
            }

            public void CustomizeTestRunnerDependencies(ObjectContainer testRunnerContainer)
            {
                
            }
        }

        public class PluginWithCustommTestRunnerDependencies : IRuntimePlugin
        {
            private readonly Action<ObjectContainer> _specificTestRunnerDependencies;

            public PluginWithCustommTestRunnerDependencies(Action<ObjectContainer> specificTestRunnerDependencies)
            {
                _specificTestRunnerDependencies = specificTestRunnerDependencies;
            }

            public void RegisterDependencies(ObjectContainer container)
            {
                
            }

            public void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
            {
                
            }

            public void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
            {
                
            }

            public void CustomizeTestRunnerDependencies(ObjectContainer testRunnerContainer)
            {
                _specificTestRunnerDependencies(testRunnerContainer);
            }
        }

        public interface ICustomDependency
        {

        }

        public class CustomDependency : ICustomDependency
        {

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

        private class CustomTestRunnerFactory : ITestRunnerFactory
        {
            public ITestRunner Create(Assembly testAssembly)
            {
                throw new NotImplementedException();
            }
        }

        class TestDefaultDependencyProvider : DefaultDependencyProvider
        {
            private readonly IRuntimePlugin pluginToReturn;

            public TestDefaultDependencyProvider(IRuntimePlugin pluginToReturn)
            {
                this.pluginToReturn = pluginToReturn;
            }

            public override void RegisterDefaults(BoDi.ObjectContainer container)
            {
                base.RegisterDefaults(container);

                var pluginLoaderStub = new Mock<IRuntimePluginLoader>();
                pluginLoaderStub.Setup(pl => pl.LoadPlugin(It.IsAny<PluginDescriptor>())).Returns(pluginToReturn);
                container.RegisterInstanceAs<IRuntimePluginLoader>(pluginLoaderStub.Object);
            }
        }

        private StringConfigProvider GetConfigWithPlugin()
        {
            return new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <configuration>
                <specFlow>
                  <plugins>
                    <add name=""MyCompany.MyPlugin"" />
                  </plugins>
                </specFlow>
              </configuration>"));
        }

        [Test]
        public void Should_be_able_to_specify_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new Mock<IRuntimePlugin>().Object);
            TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
        }

        [Test]
        public void Should_be_able_to_register_dependencies_from_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();

            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomDependency());
            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            var customDependency = container.Resolve<ICustomDependency>();
            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Test]
        public void Should_be_able_to_change_default_configuration_from_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();

            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomConfiguration(
                conf => conf.StopAtFirstError = true));
            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            var runtimeConfiguration = container.Resolve<RuntimeConfiguration>();
            runtimeConfiguration.StopAtFirstError.Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_register_further_dependencies_based_on_the_configuration()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();

            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomTestRunnerFactoryWhenStopAtFirstErrorIsTrue());

            // with default unit test provider, the plugin should not change the default factory
            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            var testRunnerFactory = container.Resolve<ITestRunnerManager>();
            testRunnerFactory.Should().BeOfType<TestRunnerManager>();

            // with StopAtFirstError == true, we should get a custom factory
            var specialConfiguratuion = new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <configuration>
                <specFlow>
                  <plugins>
                    <add name=""MyCompany.MyPlugin"" />
                  </plugins>
                  <runtime stopAtFirstError=""true"" />
                </specFlow>
              </configuration>"));
            container = TestObjectFactories.CreateDefaultGlobalContainer(specialConfiguratuion);
            var customTestRunnerFactory = container.Resolve<ITestRunnerFactory>();
            customTestRunnerFactory.Should().BeOfType<CustomTestRunnerFactory>();
        }

        [Test]
        public void Should_be_able_to_register_test_runner_dependencies_from_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustommTestRunnerDependencies(oc => oc.RegisterTypeAs<CustomDependency, ICustomDependency>()));
            var container = TestObjectFactories.CreateDefaultTestRunnerContainer(configurationHolder);
            var customDependency = container.Resolve<ICustomDependency>();

            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Test]
        public void Test_runner_dependencies_from_a_plugin_are_not_in_the_global_container()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustommTestRunnerDependencies(oc => oc.RegisterTypeAs<CustomDependency, ICustomDependency>()));
            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            
            Assert.Throws<ObjectContainerException>(() => container.Resolve<ICustomDependency>(), "Interface cannot be resolved");
        }

        [Test]
        public void Should_be_able_to_override_test_runner_registration_from_a_plugin()
        {
            StringConfigProvider configurationHolder = GetConfigWithPlugin();
            TestRunContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustommTestRunnerDependencies(oc => oc.RegisterTypeAs<CustomTraceListener, ITraceListener>()));
            var container = TestObjectFactories.CreateDefaultTestRunnerContainer(configurationHolder);
            var traceListener = container.Resolve<ITraceListener>();

            traceListener.Should().BeOfType(typeof(CustomTraceListener));
        }
    }
}
