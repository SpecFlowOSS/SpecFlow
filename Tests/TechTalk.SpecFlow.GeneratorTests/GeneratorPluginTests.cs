using System;
using BoDi;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using DefaultDependencyProvider = TechTalk.SpecFlow.Generator.DefaultDependencyProvider;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class GeneratorPluginTests
    {
        [Fact]
        public void Should_be_able_to_register_dependencies_from_a_plugin()
        {
            var pluginWithCustomDependency = new PluginWithCustomDependency();
            var generatorPluginEvents = new GeneratorPluginEvents();
            
            pluginWithCustomDependency.Initialize(generatorPluginEvents, new GeneratorPluginParameters(), new UnitTestProviderConfiguration());


            var objectContainer = new ObjectContainer();
            generatorPluginEvents.RaiseRegisterDependencies(objectContainer);


            var customDependency = objectContainer.Resolve<ICustomDependency>();
            customDependency.Should().BeOfType(typeof(CustomDependency));
        }

        [Fact]
        public void Should_be_able_to_change_default_configuration_from_a_plugin()
        {
            var pluginWithCustomConfiguration = new PluginWithCustomConfiguration(conf => conf.SpecFlowConfiguration.StopAtFirstError = true);
            var generatorPluginEvents = new GeneratorPluginEvents();

            pluginWithCustomConfiguration.Initialize(generatorPluginEvents, new GeneratorPluginParameters(), new UnitTestProviderConfiguration());
            
            var specFlowProjectConfiguration = new SpecFlowProjectConfiguration();
            generatorPluginEvents.RaiseConfigurationDefaults(specFlowProjectConfiguration);

            specFlowProjectConfiguration.SpecFlowConfiguration.StopAtFirstError.Should().BeTrue();
        }

        [Fact]
        public void Should_be_able_to_register_further_dependencies_based_on_the_configuration() //generatorPluginEvents.RaiseCustomizeDependencies();
        {
            var pluginWithCustomization = new PluginWithCustomization();
            var generatorPluginEvents = new GeneratorPluginEvents();

            pluginWithCustomization.Initialize(generatorPluginEvents, new GeneratorPluginParameters(), new UnitTestProviderConfiguration());

            var container = new ObjectContainer();
            var specFlowProjectConfiguration = new SpecFlowProjectConfiguration();
            generatorPluginEvents.RaiseCustomizeDependencies(container, specFlowProjectConfiguration);
            container.ResolveAll<ITestHeaderWriter>().Should().BeEmpty();

            specFlowProjectConfiguration.SpecFlowConfiguration.StopAtFirstError = true;
            generatorPluginEvents.RaiseCustomizeDependencies(container, specFlowProjectConfiguration);
            
            var customHeaderWriter = container.Resolve<ITestHeaderWriter>();
            customHeaderWriter.Should().BeOfType<CustomHeaderWriter>();
        }

        //[Fact]
        //public void Should_be_able_to_specify_a_plugin_with_parameters()
        //{
        //    var configurationHolder = new SpecFlowConfigurationHolder(ConfigSource.AppConfig, string.Format(@"<specFlow>
        //          <plugins>
        //            <add name=""MyCompany.MyPlugin"" parameters=""foo, bar"" />
        //          </plugins>
        //        </specFlow>"));
        //    var pluginMock = new Mock<IGeneratorPlugin>();
        //    GeneratorContainerBuilder.DefaultDependencyProvider = new TestDefaultDependencyProvider(pluginMock.Object);
        //    CreateDefaultContainer(configurationHolder);

        //    pluginMock.Verify(p => p.Initialize(It.IsAny<GeneratorPluginEvents>(), It.Is<GeneratorPluginParameters>(pp => pp.Parameters == "foo, bar"), It.IsAny<UnitTestProviderConfiguration>()));
        //}

        //private SpecFlowConfigurationHolder GetConfigWithPlugin()
        //{
        //    return new SpecFlowConfigurationHolder(ConfigSource.AppConfig, string.Format(@"<specFlow>
        //          <plugins>
        //            <add name=""MyCompany.MyPlugin"" />
        //          </plugins>
        //        </specFlow>"));
        //}

        //private IObjectContainer CreateDefaultContainer(SpecFlowConfigurationHolder configurationHolder)
        //{
        //    return new GeneratorContainerBuilder().CreateContainer(configurationHolder, new ProjectSettings(), Enumerable.Empty<GeneratorPluginInfo>());
        //}

        //class TestDefaultDependencyProvider : DefaultDependencyProvider
        //{
        //    private readonly IGeneratorPlugin pluginToReturn;

        //    public TestDefaultDependencyProvider(IGeneratorPlugin pluginToReturn)
        //    {
        //        this.pluginToReturn = pluginToReturn;
        //    }

        //    public override void RegisterDefaults(ObjectContainer container)
        //    {
        //        base.RegisterDefaults(container);

        //        var pluginLoaderStub = new Mock<IGeneratorPluginLoader>();
        //        pluginLoaderStub.Setup(pl => pl.LoadPlugin(It.IsAny<PluginDescriptor>())).Returns(pluginToReturn);
        //        container.RegisterInstanceAs<IGeneratorPluginLoader>(pluginLoaderStub.Object);
        //    }
        //}

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
            public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                generatorPluginEvents.RegisterDependencies += (sender, args) => args.ObjectContainer.RegisterTypeAs<CustomDependency, ICustomDependency>();
            }
        }

        public class PluginWithCustomization : IGeneratorPlugin
        {
            public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                generatorPluginEvents.CustomizeDependencies += (sender, args) =>
                {
                    if (args.SpecFlowProjectConfiguration.SpecFlowConfiguration.StopAtFirstError)
                        args.ObjectContainer.RegisterTypeAs<CustomHeaderWriter, ITestHeaderWriter>();
                };
            }
            
        }

        public class PluginWithCustomConfiguration : IGeneratorPlugin
        {
            private readonly Action<SpecFlowProjectConfiguration> specifyDefaults;

            public PluginWithCustomConfiguration(Action<SpecFlowProjectConfiguration> specifyDefaults)
            {
                this.specifyDefaults = specifyDefaults;
            }

            public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
            {
                generatorPluginEvents.ConfigurationDefaults += (sender, args) => { specifyDefaults(args.SpecFlowProjectConfiguration); };
            }
        }
    }
}
