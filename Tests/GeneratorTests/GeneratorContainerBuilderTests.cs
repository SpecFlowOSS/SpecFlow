using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class GeneratorContainerBuilderTests
    {
        [Test]
        public void Should_create_a_container()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());
            container.Should().NotBeNull();
        }

        [Test]
        public void Should_register_generator_configuration_with_default_config()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());
            container.Resolve<GeneratorConfiguration>().Should().NotBeNull();
        }

        [Test]
        public void Should_register_generator_with_custom_settings_when_configured()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(@"
                <specFlow>
                  <generator allowDebugGeneratedFiles=""true"" /><!-- default is false -->
                </specFlow>"), new ProjectSettings());
            container.Resolve<GeneratorConfiguration>().AllowDebugGeneratedFiles.Should().Be(true);
        }
    }
}
