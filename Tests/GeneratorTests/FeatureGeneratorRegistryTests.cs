using System;
using System.Linq;
using BoDi;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class FeatureGeneratorRegistryTests
    {
        private IObjectContainer container;

        [SetUp]
        public void Setup()
        {
            container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());
        }

        private FeatureGeneratorRegistry CreateFeatureGeneratorRegistry()
        {
            return container.Resolve<FeatureGeneratorRegistry>();
        }

        [Test]
        public void Should_create_UnitTestFeatureGenerator_with_default_setup()
        {
            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            Feature anyFeature = new Feature();
            var generator = featureGeneratorRegistry.CreateGenerator(anyFeature);

            generator.Should().BeOfType<UnitTestFeatureGenerator>();
        }

        [Test]
        public void Should_use_generic_provider_with_higher_priority()
        {
            var dummyGenerator = new Mock<IFeatureGenerator>().Object;

            var genericHighPrioProvider = new Mock<IFeatureGeneratorProvider>();
            genericHighPrioProvider.Setup(p => p.CreateGenerator(It.IsAny<Feature>())).Returns(dummyGenerator);
            genericHighPrioProvider.Setup(p => p.CanGenerate(It.IsAny<Feature>())).Returns(true); // generic
            genericHighPrioProvider.Setup(p => p.Priority).Returns(1); // high-prio

            container.RegisterInstanceAs(genericHighPrioProvider.Object, "custom");

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            Feature anyFeature = new Feature();
            var generator = featureGeneratorRegistry.CreateGenerator(anyFeature);

            generator.Should().Be(dummyGenerator);
        }

        [Test]
        public void Should_call_provider_wiht_the_given_feature()
        {
            var dummyGenerator = new Mock<IFeatureGenerator>().Object;

            var genericHighPrioProvider = new Mock<IFeatureGeneratorProvider>();
            genericHighPrioProvider.Setup(p => p.CreateGenerator(It.IsAny<Feature>())).Returns(dummyGenerator);
            genericHighPrioProvider.Setup(p => p.CanGenerate(It.IsAny<Feature>())).Returns(true); // generic
            genericHighPrioProvider.Setup(p => p.Priority).Returns(1); // high-prio

            container.RegisterInstanceAs(genericHighPrioProvider.Object, "custom");

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            Feature theFeature = new Feature();
            featureGeneratorRegistry.CreateGenerator(theFeature);

            genericHighPrioProvider.Verify(p => p.CreateGenerator(theFeature), Times.Once());
        }

        [Test]
        public void Should_skip_high_priority_provider_when_not_applicable()
        {
            var dummyGenerator = new Mock<IFeatureGenerator>().Object;

            Feature theFeature = new Feature();

            var genericHighPrioProvider = new Mock<IFeatureGeneratorProvider>();
            genericHighPrioProvider.Setup(p => p.CreateGenerator(It.IsAny<Feature>())).Returns(dummyGenerator);
            genericHighPrioProvider.Setup(p => p.CanGenerate(theFeature)).Returns(false); // not applicable for aFeature
            genericHighPrioProvider.Setup(p => p.Priority).Returns(1); // high-prio

            container.RegisterInstanceAs(genericHighPrioProvider.Object, "custom");

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theFeature);

            generator.Should().BeOfType<UnitTestFeatureGenerator>();
        }

        [Test]
        public void Should_FeatureGeneratorRegistry_be_registered_as_IFeatureGeneratorRegistry_by_default()
        {
            var testContainer = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());

            var registry = testContainer.Resolve<IFeatureGeneratorRegistry>();

            registry.Should().BeOfType<FeatureGeneratorRegistry>();
        }

        private class TestTagFilteredFeatureGeneratorProvider : TagFilteredFeatureGeneratorProvider
        {
            static public IFeatureGenerator DummyGenerator = new Mock<IFeatureGenerator>().Object;

            public TestTagFilteredFeatureGeneratorProvider(ITagFilterMatcher tagFilterMatcher, string registeredName) : base(tagFilterMatcher, registeredName)
            {
            }

            public override IFeatureGenerator CreateGenerator(Feature feature)
            {
                return DummyGenerator;
            }
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_applied_for_registered_tag_name()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("mytag");

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("mytag"));

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theFeature);

            generator.Should().Be(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_applied_for_registered_tag_name_with_at()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("@mytag");

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("mytag"));

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theFeature);

            generator.Should().Be(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_not_be_applied_for_feature_with_other_tgas()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("mytag");

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("othertag"));

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theFeature);

            generator.Should().NotBe(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_not_be_applied_for_feature_with_no_tgas()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("mytag");

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags();

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theFeature);

            generator.Should().NotBe(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }
    }
}
