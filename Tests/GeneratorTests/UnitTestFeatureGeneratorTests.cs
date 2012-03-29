using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using FluentAssertions;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public abstract class UnitTestFeatureGeneratorTestsBase
    {
        protected IObjectContainer container;
        protected Mock<IUnitTestGeneratorProvider> unitTestGeneratorProviderMock;

        [SetUp]
        public void Setup()
        {
            SetupInternal();
        }

        protected virtual void SetupInternal()
        {
            container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());
            unitTestGeneratorProviderMock = new Mock<IUnitTestGeneratorProvider>();
            container.RegisterInstanceAs(unitTestGeneratorProviderMock.Object);
        }

        protected IFeatureGenerator CreateUnitTestFeatureGenerator()
        {
            return container.Resolve<UnitTestFeatureGeneratorProvider>().CreateGenerator(new Feature());
        }

        protected Feature CreateFeature(string[] tags = null)
        {
            tags = tags ?? new string[0];

            Scenario scenario1 = new Scenario("Scenario", "scenario1 title", "", new Tags(), new ScenarioSteps());

            return new Feature("feature", "title", new Tags(tags.Select(t => new Tag(t)).ToArray()), "desc", null, new Scenario[] {scenario1}, new Comment[0]);
        }

        protected void GenerateFeature(IFeatureGenerator generator, Feature feature)
        {
            generator.GenerateUnitTestFixture(feature, "dummy", "dummyNS");
        }
    }

    [TestFixture]
    public class UnitTestFeatureGeneratorTests : UnitTestFeatureGeneratorTestsBase
    {
        [Test]
        public void Should_pass_feature_tags_as_test_class_category()
        {
            var generator = CreateUnitTestFeatureGenerator();
            string[] generatedCats = new string[0];
            unitTestGeneratorProviderMock.Setup(ug => ug.SetTestClassCategories(It.IsAny<TestClassGenerationContext>(), It.IsAny<IEnumerable<string>>()))
                .Callback((TestClassGenerationContext ctx, IEnumerable<string> cats) => generatedCats = cats.ToArray());

            Feature theFeature = CreateFeature(new string[] { "foo", "bar" });

            GenerateFeature(generator, theFeature);

            generatedCats.Should().Equal(new string[] {"foo", "bar"});
        }

        [Test]
        public void Should_pass_scenario_tags_as_test_method_category()
        {
            var generator = CreateUnitTestFeatureGenerator();
            string[] generatedCats = new string[0];
            unitTestGeneratorProviderMock.Setup(ug => ug.SetTestMethodCategories(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>(), It.IsAny<IEnumerable<string>>()))
                .Callback((TestClassGenerationContext ctx, CodeMemberMethod _, IEnumerable<string> cats) => generatedCats = cats.ToArray());

            Feature theFeature = CreateFeature();
            theFeature.Scenarios[0].Tags = new Tags(new Tag("foo"), new Tag("bar"));


            GenerateFeature(generator, theFeature);

            generatedCats.Should().Equal(new string[] {"foo", "bar"});
        }

        [Test]
        public void Should_not_pass_feature_tags_as_test_method_category()
        {
            var generator = CreateUnitTestFeatureGenerator();
            string[] generatedCats = new string[0];
            unitTestGeneratorProviderMock.Setup(ug => ug.SetTestMethodCategories(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>(), It.IsAny<IEnumerable<string>>()))
                .Callback((TestClassGenerationContext ctx, CodeMemberMethod _, IEnumerable<string> cats) => generatedCats = cats.ToArray());

            Feature theFeature = CreateFeature(new string[] { "featuretag" });
            theFeature.Scenarios[0].Tags = new Tags(new Tag("foo"), new Tag("bar"));

            GenerateFeature(generator, theFeature);

            generatedCats.Should().Equal(new string[] {"foo", "bar"});
        }

        [Test]
        public void Should_not_pass_decorated_feature_tag_as_test_class_category()
        {
            var decoratorMock = DecoratorRegistryTests.CreateTestClassTagDecoratorMock("decorated");
            container.RegisterInstanceAs(decoratorMock.Object, "decorated");

            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature(new string[] { "decorated", "other" });

            GenerateFeature(generator, theFeature);

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassCategories(It.IsAny<TestClassGenerationContext>(), It.Is<IEnumerable<string>>(cats => !cats.Contains("decorated"))));
        }

        [Test]
        public void Should_not_pass_decorated_scenario_tag_as_test_method_category()
        {
            var decoratorMock = DecoratorRegistryTests.CreateTestMethodTagDecoratorMock("decorated");
            container.RegisterInstanceAs(decoratorMock.Object, "decorated");

            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature();
            theFeature.Scenarios[0].Tags = new Tags(new Tag("decorated"), new Tag("other"));

            GenerateFeature(generator, theFeature);

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodCategories(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>(), It.Is<IEnumerable<string>>(cats => !cats.Contains("decorated"))));
        }
    }
}
