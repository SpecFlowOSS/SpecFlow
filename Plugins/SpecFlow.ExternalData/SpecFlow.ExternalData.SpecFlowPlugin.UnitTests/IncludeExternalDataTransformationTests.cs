using System;
using System.Collections.Generic;
using System.IO;
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
        private const string DOCUMENT_PATH = @"C:\Temp\Sample.feature";
        private ExternalDataSpecification _specification;
        private Mock<ISpecificationProvider> _specificationProviderMock;

        public IncludeExternalDataTransformationTests()
        {
            _specificationProviderMock = new Mock<ISpecificationProvider>(MockBehavior.Default);
            _specificationProviderMock.Setup(sp => sp.GetSpecification(It.IsAny<IEnumerable<Tag>>(), It.IsAny<string>()))
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

        private SpecFlowDocument CreateSpecFlowDocument(IHasLocation child)
        {
            return new(new SpecFlowFeature(new Tag[0], null, null, "Feature", "Sample feature", "", new IHasLocation[] { child }), new Comment[0], 
                       new SpecFlowDocumentLocation(DOCUMENT_PATH));
        }

        private ScenarioOutline CreateScenarioOutline()
        {
            return new(
                new Tag[0],
                null,
                "Scenario Outline",
                "SO 1",
                null,
                new[] { new Step(null, "Given ", "the customer has <product>", null) },
                new[] { new Examples(new Tag[0], null, "Examples", "", "", new TableRow(null, new[] { new TableCell(null, "product") }), new TableRow[0]) });
        }

        [Fact]
        public void Should_include_external_data_to_scenario_outline()
        {
            var scenarioOutline = CreateScenarioOutline();
            var document = CreateSpecFlowDocument(scenarioOutline);
            _specification = new ExternalDataSpecification(new DataValue(CreateProductDataList()));

            var sut = CreateSut();

            var result = sut.TransformDocument(document);
            
            var transformedOutline = result.Feature.Children.OfType<ScenarioOutline>().FirstOrDefault();
            Assert.NotNull(transformedOutline);
            var examples = transformedOutline.Examples.Last();
            Assert.Equal(3, examples.TableBody.Count());
        }

        [Fact]
        public void Should_provide_path_of_source_file_for_SpecificationProvider()
        {
            var scenarioOutline = CreateScenarioOutline();
            var document = CreateSpecFlowDocument(scenarioOutline);
            string capturedSourceFilePath = "n/a";
            _specificationProviderMock = new Mock<ISpecificationProvider>();
            _specificationProviderMock.Setup(sp => sp.GetSpecification(It.IsAny<IEnumerable<Tag>>(), It.IsAny<string>()))
                                      .Returns((IEnumerable<Tag> _, string sourceFilePath) =>
                                      {
                                          capturedSourceFilePath = sourceFilePath;
                                          return null;
                                      });


            var sut = CreateSut();

            var result = sut.TransformDocument(document);

            Assert.Equal(DOCUMENT_PATH, capturedSourceFilePath);
        }
    }
}
