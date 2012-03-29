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

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class IgnoredTestGeneratorTests
    {
        private IObjectContainer container;
        private Mock<IUnitTestGeneratorProvider> unitTestGeneratorProviderMock;
            
        [SetUp]
        public void Setup()
        {
            container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());
            unitTestGeneratorProviderMock = new Mock<IUnitTestGeneratorProvider>();
            container.RegisterInstanceAs(unitTestGeneratorProviderMock.Object);
        }

        private IFeatureGenerator CreateUnitTestFeatureGenerator()
        {
            return container.Resolve<UnitTestFeatureGeneratorProvider>().CreateGenerator(new Feature());
        }

        private Feature CreateFeature(string[] tags = null)
        {
            tags = tags ?? new string[0];

            Scenario scenario1 = new Scenario("Scenario", "scenario1 title", "", new Tags(), new ScenarioSteps());

            return new Feature("feature", "title", new Tags(tags.Select(t => new Tag(t)).ToArray()), "desc", null, new Scenario[] {scenario1}, new Comment[0]);
        }

        [Test]
        public void Should_call_SetTestClassIgnore_when_feature_ignored()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature(new string[] {"ignore"});

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassIgnore(It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_support_case_insensitive_ignore_tag_on_feature()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature(new string[] {"IgnoRe"});

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassIgnore(It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_not_call_SetTestMethodIgnore_when_feature_ignored()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature(new string[] {"ignore"});

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()), Times.Never());
        }

        [Test]
        public void Should_call_SetTestMethodIgnore_when_scenario_ignored()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature();
            theFeature.Scenarios[0].Tags = new Tags(new Tag("ignore"));

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }

        [Test]
        public void Should_support_case_insensitive_ignore_tag_on_scenario()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature();
            theFeature.Scenarios[0].Tags = new Tags(new Tag("IgnoRe"));

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }

        [Test]
        public void Should_not_pass_ignore_as_test_class_category()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature(new string[] {"ignore", "other"});

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassCategories(It.IsAny<TestClassGenerationContext>(), It.Is<IEnumerable<string>>(cats => !cats.Contains("ignore"))));
        }

        [Test]
        public void Should_not_pass_ignore_as_test_category()
        {
            var generator = CreateUnitTestFeatureGenerator();

            Feature theFeature = CreateFeature();
            theFeature.Scenarios[0].Tags = new Tags(new Tag("ignore"), new Tag("other"));

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodCategories(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>(), It.Is<IEnumerable<string>>(cats => !cats.Contains("ignore"))));
        }
    }
}
