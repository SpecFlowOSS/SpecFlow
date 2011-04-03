using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using Should;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace GeneratorTests
{
    [TestFixture]
    public class GeneratorContainerBuilderTests
    {
        [Test]
        public void Should_create_a_container()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder());
            container.ShouldNotBeNull();
        }

        [Test]
        public void Should_register_generator_configuration_with_default_config()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder());
            container.Resolve<GeneratorConfiguration>().ShouldNotBeNull();
        }

        [Test]
        public void Should_register_generator_with_custom_settings_when_configured()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(@"
                <specFlow>
                  <generator allowDebugGeneratedFiles=""true"" /><!-- default is false -->
                </specFlow>"));
            container.Resolve<GeneratorConfiguration>().AllowDebugGeneratedFiles.ShouldBeTrue();
        }
    }
}
