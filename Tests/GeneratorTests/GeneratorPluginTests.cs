using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using DefaultDependencyProvider = TechTalk.SpecFlow.Generator.DefaultDependencyProvider;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class GeneratorPluginTests
    {
        [Test]
        public void Should_be_able_to_specify_a_plugin()
        {
            var configurationHolder = GetConfigWithPlugin();
            GeneratorContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new Mock<IGeneratorPlugin>().Object);
            CreateDefaultContainer(configurationHolder);
        }

        [Test]
        public void Should_be_able_to_register_dependencies_from_a_plugin()
        {
            var configurationHolder = GetConfigWithPlugin();

            GeneratorContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomDependency());
            var container = CreateDefaultContainer(configurationHolder);
            var customDependency = container.Resolve<ICustomDependency>();
            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Test]
        public void Should_be_able_to_change_default_configuration_from_a_plugin()
        {
            var configurationHolder = GetConfigWithPlugin();

            GeneratorContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomConfiguration(conf => conf.RuntimeConfiguration.StopAtFirstError = true));
            var container = CreateDefaultContainer(configurationHolder);
            var runtimeConfiguration = container.Resolve<SpecFlowProjectConfiguration>();
            runtimeConfiguration.RuntimeConfiguration.StopAtFirstError.Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_register_further_dependencies_based_on_the_configuration()
        {
            var configurationHolder = GetConfigWithPlugin();

            GeneratorContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(new PluginWithCustomization());

            // with default settings, the plugin should not change the header writer
            var container = CreateDefaultContainer(configurationHolder);
            var testHeaderWriter = container.Resolve<ITestHeaderWriter>();
            testHeaderWriter.Should().BeOfType<TestHeaderWriter>();

            // with StopAtFirstError == true, we should get a custom factory
            var specialConfiguratuion = new SpecFlowConfigurationHolder(string.Format(@"<specFlow>
                  <plugins>
                    <add name=""MyCompany.MyPlugin"" />
                  </plugins>
                  <runtime stopAtFirstError=""true"" />
                </specFlow>"));
            container = CreateDefaultContainer(specialConfiguratuion);
            var customHeaderWriter = container.Resolve<ITestHeaderWriter>();
            customHeaderWriter.Should().BeOfType<CustomHeaderWriter>();
        }

        private SpecFlowConfigurationHolder GetConfigWithPlugin()
        {
            return new SpecFlowConfigurationHolder(string.Format(@"<specFlow>
                  <plugins>
                    <add name=""MyCompany.MyPlugin"" />
                  </plugins>
                </specFlow>"));
        }

        private IObjectContainer CreateDefaultContainer(SpecFlowConfigurationHolder configurationHolder)
        {
            return GeneratorContainerBuilder.CreateContainer(configurationHolder, new ProjectSettings());
        }

        class TestDefaultDependencyProvider : DefaultDependencyProvider
        {
            private readonly IGeneratorPlugin pluginToReturn;

            public TestDefaultDependencyProvider(IGeneratorPlugin pluginToReturn)
            {
                this.pluginToReturn = pluginToReturn;
            }

            public override void RegisterDefaults(ObjectContainer container)
            {
                base.RegisterDefaults(container);

                var pluginLoaderStub = new Mock<IGeneratorPluginLoader>();
                pluginLoaderStub.Setup(pl => pl.LoadPlugin(It.IsAny<PluginDescriptor>())).Returns(pluginToReturn);
                container.RegisterInstanceAs<IGeneratorPluginLoader>(pluginLoaderStub.Object);
            }
        }

        public interface ICustomDependency
        {

        }

        public class CustomDependency : ICustomDependency
        {

        }

        public class CustomHeaderWriter : ITestHeaderWriter
        {
            public Version DetectGeneratedTestVersion(string generatedTestContent)
            {
                throw new NotImplementedException();
            }
        }

        public class PluginWithCustomDependency : IGeneratorPlugin
        {
            public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
            {
                //runtimePluginEvents.RegisterGlobalDependencies += (sender, args) => args.ObjectContainer.RegisterTypeAs<CustomDependency, ICustomDependency>();
            }

            public void RegisterDependencies(ObjectContainer container)
            {
                container.RegisterTypeAs<CustomDependency, ICustomDependency>();
            }

            public void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration specFlowProjectConfiguration)
            {
                //nop
            }

            public void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration)
            {
                //nop
            }
        }

        public class PluginWithCustomization : IGeneratorPlugin
        {
            public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
            {
                //runtimePluginEvents.RegisterGlobalDependencies += (sender, args) => args.ObjectContainer.RegisterTypeAs<CustomDependency, ICustomDependency>();
            }

            public void RegisterDependencies(ObjectContainer container)
            {
            }

            public void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration specFlowProjectConfiguration)
            {
                if (specFlowProjectConfiguration.RuntimeConfiguration.StopAtFirstError)
                    container.RegisterTypeAs<CustomHeaderWriter, ITestHeaderWriter>();
            }

            public void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration)
            {
                //nop
            }
        }

        public class PluginWithCustomConfiguration : IGeneratorPlugin
        {
            private readonly Action<SpecFlowProjectConfiguration> specifyDefaults;

            public PluginWithCustomConfiguration(Action<SpecFlowProjectConfiguration> specifyDefaults)
            {
                this.specifyDefaults = specifyDefaults;
            }

            public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
            {
                //runtimePluginEvents.ConfigurationDefaults += (sender, args) => { specifyDefaults(args.RuntimeConfiguration); };
            }

            public void RegisterDependencies(ObjectContainer container)
            {
                //nop
            }

            public void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration specFlowProjectConfiguration)
            {
                //nop
            }

            public void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration)
            {
                specifyDefaults(specFlowConfiguration);
            }
        }
    }
}
