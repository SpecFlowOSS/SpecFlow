using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class TestGeneratorFactoryTests : TestGeneratorTestsBase
    {
        private TestGeneratorFactory factory;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            factory = new TestGeneratorFactory();
        }

        [Test]
        public void GetGeneratorVersion_should_return_a_version()
        {
            factory.GetGeneratorVersion().Should().NotBeNull();
        }

        [Test]
        public void Should_be_able_to_create_generator_with_default_config()
        {
            net35CSProjectSettings.ConfigurationHolder = new SpecFlowConfigurationHolder(ConfigSource.Default, null);
            factory.CreateGenerator(net35CSProjectSettings).Should().NotBeNull();
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

        [Test]
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
            var generator = factory.CreateGenerator(projectSettings);
            generator.Should().BeOfType<DummyGenerator>();
        }
    }
}
