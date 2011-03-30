using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using Should;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace GeneratorTests
{
    [TestFixture]
    public class TestGeneratorFactoryTests
    {
        private TestGeneratorFactory factory;

        [SetUp]
        public void Setup()
        {
            factory = new TestGeneratorFactory();
        }

        [Test]
        public void GetGeneratorVersion_should_return_a_version()
        {
            factory.GetGeneratorVersion().ShouldNotBeNull();
        }

        [Test]
        public void Should_be_able_to_create_generator_with_default_config()
        {
            factory.CreateGenerator(new SpecFlowConfigurationHolder()).ShouldNotBeNull();
        }

        private class DummyGenerator : ITestGenerator
        {
            public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Should_create_custom_generator_when_configured_so()
        {
            var generator = factory.CreateGenerator(new SpecFlowConfigurationHolder(string.Format(@"
                <specFlow>
                  <dependencies>
                    <register type=""{0}"" as=""{1}""/>
                  </dependencies>
                </specFlow>",
                typeof(DummyGenerator).AssemblyQualifiedName,
                typeof(ITestGenerator).AssemblyQualifiedName)));

            generator.ShouldBeType(typeof(DummyGenerator));
        }
    }
}
