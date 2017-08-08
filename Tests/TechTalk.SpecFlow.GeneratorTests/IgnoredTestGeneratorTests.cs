using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using Gherkin.Ast;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;

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
            container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.Default, null), new ProjectSettings());
            unitTestGeneratorProviderMock = new Mock<IUnitTestGeneratorProvider>();
            container.RegisterInstanceAs(unitTestGeneratorProviderMock.Object);
        }

        private IFeatureGenerator CreateUnitTestFeatureGenerator()
        {
            return container.Resolve<UnitTestFeatureGeneratorProvider>().CreateGenerator(ParserHelper.CreateAnyDocument());
        }

        [Test]
        public void Should_call_SetTestClassIgnore_when_feature_ignored()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(new string[] {"ignore"});

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassIgnore(It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_support_case_insensitive_ignore_tag_on_feature()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(new string[] {"IgnoRe"});

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassIgnore(It.IsAny<TestClassGenerationContext>()));
        }

        [Test]
        public void Should_not_call_SetTestMethodIgnore_when_feature_ignored()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(new string[] {"ignore"});

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()), Times.Never());
        }

        [Test]
        public void Should_call_SetTestMethodIgnore_when_scenario_ignored()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(scenarioTags: new []{"ignore"});

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }

        [Test]
        public void Should_support_case_insensitive_ignore_tag_on_scenario()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(scenarioTags: new[] { "IgnoRe" });

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }

        [Test]
        public void Should_not_pass_ignore_as_test_class_category()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(new string[] {"ignore", "other"});

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestClassCategories(It.IsAny<TestClassGenerationContext>(), It.Is<IEnumerable<string>>(cats => !cats.Contains("ignore"))));
        }

        [Test]
        public void Should_not_pass_ignore_as_test_category()
        {
            var generator = CreateUnitTestFeatureGenerator();

            SpecFlowDocument theDocument = ParserHelper.CreateDocument(scenarioTags: new[] { "ignore", "other" });

            generator.GenerateUnitTestFixture(theDocument, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodCategories(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>(), It.Is<IEnumerable<string>>(cats => !cats.Contains("ignore"))));
        }

        [Test]
        public void Should_call_SetTestMethodIgnore_when_scenario_outline_ignored()
        {
            unitTestGeneratorProviderMock.Setup(p=>p.GetTraits()).Returns(UnitTestGeneratorTraits.RowTests); // e.g. xunit 
            var generator = CreateUnitTestFeatureGenerator();

            var theFeature = ParserHelper.CreateDocumentWithScenarioOutline(scenarioOutlineTags: new[] {"ignore"});

            generator.GenerateUnitTestFixture(theFeature, "dummy", "dummyNS");

            unitTestGeneratorProviderMock.Verify(ug => ug.SetTestMethodIgnore(It.IsAny<TestClassGenerationContext>(), It.IsAny<CodeMemberMethod>()));
        }
    }
}
