using System;
using System.Collections.Generic;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests.DataSources.Selectors
{
    public class DataSourceSelectorTests
    {
        private DataSourceSelector CreateSut(string expression) => 
            new DataSourceSelectorParser().Parse(expression);

        [Fact]
        public void Should_evaluate_simple_field_selector()
        {
            var record = new DataRecord(
                new Dictionary<string, string>
                {
                    { "field1", "value1" },
                    { "field2", "value2" },
                });

            var sut = CreateSut("field2");

            var result = sut.Evaluate(new DataValue(record));
            
            Assert.NotNull(result);
            Assert.Equal("value2", result.AsString());
        }

        [Fact]
        public void Handles_when_the_requested_field_was_not_provided()
        {
            var record = new DataRecord(
                new Dictionary<string, string>
                {
                    { "field1", "value1" },
                    { "field2", "value2" },
                });
            var sut = CreateSut("no-such-field");

            Assert.Contains("no-such-field",
                Assert.Throws<ExternalDataPluginException>(() =>
                    sut.Evaluate(new DataValue(record))).Message);
        }

        [Theory]
        [InlineData("product name", "product_name")]
        [InlineData("product_name", "product name")]
        public void Should_transform_underscores_to_spaces_when_matching(string fieldName, string selectorFieldName)
        {
            var record = new DataRecord(
                new Dictionary<string, string>
                {
                    { fieldName, "Chocolate" },
                });

            var sut = CreateSut(selectorFieldName);

            var result = sut.Evaluate(new DataValue(record));

            Assert.NotNull(result);
            Assert.Equal("Chocolate", result.AsString());
        }
    }
}
