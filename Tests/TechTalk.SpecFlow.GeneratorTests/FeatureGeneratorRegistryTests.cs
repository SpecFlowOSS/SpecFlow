﻿using System;
using System.Linq;
using BoDi;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

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

            var anyDocument = ParserHelper.CreateAnyDocument();
            var generator = featureGeneratorRegistry.CreateGenerator(anyDocument);

            generator.Should().BeOfType<UnitTestFeatureGenerator>();
        }

        [Test]
        public void Should_use_generic_provider_with_higher_priority()
        {
            var dummyGenerator = new Mock<IFeatureGenerator>().Object;

            var genericHighPrioProvider = new Mock<IFeatureGeneratorProvider>();
            genericHighPrioProvider.Setup(p => p.CreateGenerator(It.IsAny<SpecFlowDocument>())).Returns(dummyGenerator);
            genericHighPrioProvider.Setup(p => p.CanGenerate(It.IsAny<SpecFlowDocument>())).Returns(true); // generic
            genericHighPrioProvider.Setup(p => p.Priority).Returns(1); // high-prio

            container.RegisterInstanceAs(genericHighPrioProvider.Object, "custom");

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var anyFeature = ParserHelper.CreateAnyDocument();
            var generator = featureGeneratorRegistry.CreateGenerator(anyFeature);

            generator.Should().Be(dummyGenerator);
        }

        [Test]
        public void Should_call_provider_wiht_the_given_feature()
        {
            var dummyGenerator = new Mock<IFeatureGenerator>().Object;

            var genericHighPrioProvider = new Mock<IFeatureGeneratorProvider>();
            genericHighPrioProvider.Setup(p => p.CreateGenerator(It.IsAny<SpecFlowDocument>())).Returns(dummyGenerator);
            genericHighPrioProvider.Setup(p => p.CanGenerate(It.IsAny<SpecFlowDocument>())).Returns(true); // generic
            genericHighPrioProvider.Setup(p => p.Priority).Returns(1); // high-prio

            container.RegisterInstanceAs(genericHighPrioProvider.Object, "custom");

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            SpecFlowDocument theDocument = ParserHelper.CreateAnyDocument();
            featureGeneratorRegistry.CreateGenerator(theDocument);

            genericHighPrioProvider.Verify(p => p.CreateGenerator(theDocument), Times.Once());
        }

        [Test]
        public void Should_skip_high_priority_provider_when_not_applicable()
        {
            var dummyGenerator = new Mock<IFeatureGenerator>().Object;

            SpecFlowDocument theDocument = ParserHelper.CreateAnyDocument();

            var genericHighPrioProvider = new Mock<IFeatureGeneratorProvider>();
            genericHighPrioProvider.Setup(p => p.CreateGenerator(It.IsAny<SpecFlowDocument>())).Returns(dummyGenerator);
            genericHighPrioProvider.Setup(p => p.CanGenerate(theDocument)).Returns(false); // not applicable for aFeature
            genericHighPrioProvider.Setup(p => p.Priority).Returns(1); // high-prio

            container.RegisterInstanceAs(genericHighPrioProvider.Object, "custom");

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theDocument);

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

            public override IFeatureGenerator CreateGenerator(SpecFlowDocument feature)
            {
                return DummyGenerator;
            }
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_applied_for_registered_tag_name()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("mytag");

            SpecFlowDocument theDocument = ParserHelper.CreateAnyDocument(tags: new[] {"mytag"});

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theDocument);

            generator.Should().Be(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_applied_for_registered_tag_name_with_at()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("@mytag");

            SpecFlowDocument theDocument = ParserHelper.CreateAnyDocument(tags: new[] { "mytag" });

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theDocument);

            generator.Should().Be(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_not_be_applied_for_feature_with_other_tgas()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("mytag");

            SpecFlowDocument theDocument = ParserHelper.CreateAnyDocument(tags: new[] { "othertag" });

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theDocument);

            generator.Should().NotBe(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }

        [Test]
        public void Should_TagFilteredFeatureGeneratorProvider_not_be_applied_for_feature_with_no_tgas()
        {
            container.RegisterTypeAs<TestTagFilteredFeatureGeneratorProvider, IFeatureGeneratorProvider>("mytag");

            SpecFlowDocument theDocument = ParserHelper.CreateAnyDocument();

            var featureGeneratorRegistry = CreateFeatureGeneratorRegistry();

            var generator = featureGeneratorRegistry.CreateGenerator(theDocument);

            generator.Should().NotBe(TestTagFilteredFeatureGeneratorProvider.DummyGenerator);
        }
    }
}
