using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class TestGeneratorFactoryTests : TestGeneratorTestsBase
    {
        private readonly TestGeneratorFactory _factory;

        public TestGeneratorFactoryTests()
        {
            _factory = new TestGeneratorFactory();
        }

        [Fact]
        public void GetGeneratorVersion_should_return_a_version()
        {
            _factory.GetGeneratorVersion().Should().NotBeNull();
        }

        [Fact]
        public void Should_be_able_to_create_generator_with_default_config()
        {
            net35CSProjectSettings.ConfigurationHolder = new SpecFlowConfigurationHolder(ConfigSource.Default, null);
            _factory.CreateGenerator(net35CSProjectSettings, Enumerable.Empty<GeneratorPluginInfo>()).Should().NotBeNull();
        }

        private class DummyGenerator : ITestGenerator
        {
            public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
            {
                throw new NotImplementedException();
            }

            public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
            {
                throw new NotImplementedException();
            }

            public string GetTestFullPath(FeatureFileInput featureFileInput)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                //nop;
            }
        }

        [Fact]
        public void Should_create_custom_generator_when_configured_so()
        {
            var configurationHolder = new SpecFlowConfigurationHolder(ConfigSource.AppConfig, string.Format(@"
                <specFlow>
                  <generator>  
                  <dependencies>
                    <register type=""{0}"" as=""{1}""/>
                  </dependencies>
                  </generator>
                </specFlow>",
                typeof(DummyGenerator).AssemblyQualifiedName,
                typeof(ITestGenerator).AssemblyQualifiedName));

            var projectSettings = net35CSProjectSettings;
            projectSettings.ConfigurationHolder = configurationHolder;
            var generator = _factory.CreateGenerator(projectSettings, Enumerable.Empty<GeneratorPluginInfo>());
            generator.Should().BeOfType<DummyGenerator>();
        }
    }
}
