using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using Moq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;
using SpecFlow.ExternalData.SpecFlowPlugin.Transformation;
using TechTalk.SpecFlow.Parser;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class IncludeExternalDataTransformationTests
    {
        private ExternalDataSpecification _specification;
        private readonly Mock<ISpecificationProvider> _specificationProviderMock;

        public IncludeExternalDataTransformationTests()
        {
            _specificationProviderMock = new Mock<ISpecificationProvider>(MockBehavior.Default);
            _specificationProviderMock.Setup(sp => sp.GetSpecification(It.IsAny<IEnumerable<Tag>>()))
                                      .Returns(() => _specification);
        }

        private IncludeExternalDataTransformation CreateSut() => new(_specificationProviderMock.Object);

        private DataList CreateProductDataList()
        {
            return new()
            {
                Items =
                {
                    new DataValue(new DataRecord(new Dictionary<string, string> { {"product", "Chocolate" }, {"price", "2.5"} })),
                    new DataValue(new DataRecord(new Dictionary<string, string> { {"product", "Apple" }, {"price", "1.0"} })),
                    new DataValue(new DataRecord(new Dictionary<string, string> { {"product", "Orange" }, {"price", "1.2"} })),
                }
            };
        }

        [Fact]
        public void Should_return_a_document()
        {
            var document = new SpecFlowDocument(new SpecFlowFeature(new Tag[0], null, null, "Feature", "Sample feature", "", new IHasLocation[0]), new Comment[0], null);
            _specification = new ExternalDataSpecification(new DataValue(null));

            var sut = CreateSut();

            var result = sut.TransformDocument(document);
            
            Assert.NotNull(result);
        }

        [Fact]
        public void Should_include_external_data_to_scenario_outline()
        {
            var scenarioOutline = new ScenarioOutline(
                new Tag[0],
                null,
                "Scenario Outline",
                "SO 1",
                null,
                new[] { new Step(null, "Given ", "the customer has <product>", null) },
                new[] { new Examples(new Tag[0], null, "Examples", "", "", new TableRow(null, new[] { new TableCell(null, "product") }), new TableRow[0]) });
            var document = new SpecFlowDocument(new SpecFlowFeature(new Tag[0], null, null, "Feature", "Sample feature", "", new IHasLocation[] { scenarioOutline }), new Comment[0], null);
            _specification = new ExternalDataSpecification(new DataValue(CreateProductDataList()));

            var sut = CreateSut();

            var result = sut.TransformDocument(document);
            
            var transformedOutline = result.Feature.Children.OfType<ScenarioOutline>().FirstOrDefault();
            Assert.NotNull(transformedOutline);
            var examples = transformedOutline.Examples.Last();
            Assert.Equal(3, examples.TableBody.Count());
        }
    }
}
