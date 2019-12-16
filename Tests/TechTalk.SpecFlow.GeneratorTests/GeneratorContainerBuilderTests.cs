using System.Linq;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class GeneratorContainerBuilderTests
    {
        [Fact]
        public void Should_create_a_container()
        {
            var container = new GeneratorContainerBuilder().CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.Default, null), new ProjectSettings(), Enumerable.Empty<GeneratorPluginInfo>());
            container.Should().NotBeNull();
        }

        [Fact]
        public void Should_register_generator_configuration_with_default_config()
        {
            var container = new GeneratorContainerBuilder().CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.Default, null), new ProjectSettings(), Enumerable.Empty<GeneratorPluginInfo>());
            container.Resolve<Configuration.SpecFlowConfiguration>().Should().NotBeNull();
        }

        [Fact]
        public void Should_register_generator_with_custom_settings_when_configured()
        {
            var container = new GeneratorContainerBuilder().CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.AppConfig, @"
                <specFlow>
                  <generator allowDebugGeneratedFiles=""true"" /><!-- default is false -->
                </specFlow>"), new ProjectSettings(), Enumerable.Empty<GeneratorPluginInfo>());
            container.Resolve<SpecFlowConfiguration>().AllowDebugGeneratedFiles.Should().Be(true);
        }
    }
}
