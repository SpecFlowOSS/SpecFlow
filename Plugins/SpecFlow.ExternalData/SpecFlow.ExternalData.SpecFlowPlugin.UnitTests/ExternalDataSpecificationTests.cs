using System;
using System.Collections.Generic;
using System.Linq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class ExternalDataSpecificationTests
    {
        private DataTable CreateProductDataList()
        {
            return new(new []{ "product", "price", "color"})
            {
                Items =
                {
                    new DataRecord(new Dictionary<string, string> { {"product", "Chocolate" }, {"price", "2.5"}, {"color", "brown"} }),
                    new DataRecord(new Dictionary<string, string> { {"product", "Apple" }, {"price", "1.0"}, {"color", "red"} }),
                    new DataRecord(new Dictionary<string, string> { {"product", "Orange" }, {"price", "1.2"}, {"color", "orange" } }),
                }
            };
        }

        private ExternalDataSpecification CreateSut(Dictionary<string, string> fields = null)
        {
            var selectorParser = new DataSourceSelectorParser();
            return new(new DataSource(CreateProductDataList()), 
                fields?.ToDictionary(
                    f => f.Key, 
                    f => selectorParser.Parse(f.Value)));
        }

        [Fact]
        public void Should_return_DataTable_fields_if_expected_header_names_not_provided()
        {
            var sut = CreateSut();
            
            var result = 
                sut.GetExampleRecords(null);
            
            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(new[] { "product", "price", "color" }, result.Header);
            Assert.Equal("Chocolate", result.Items[0].Fields["product"].Value);
            Assert.Equal("brown", result.Items[0].Fields["color"].Value);
        }

        [Fact]
        public void Should_return_requested_fields_from_data_table_in_the_same_order()
        {
            var sut = CreateSut();
            var headerNames = new[] { "color", "product" };
            
            var result = 
                sut.GetExampleRecords(headerNames);
            
            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(headerNames, result.Header);
            Assert.Equal("Chocolate", result.Items[0].Fields["product"].Value);
            Assert.Equal("brown", result.Items[0].Fields["color"].Value);
        }

        [Fact]
        public void Should_support_field_renames()
        {
            var sut = CreateSut(new Dictionary<string,string>
            {
                { "product name", "product" },
                { "price-in-EUR", "price" },
            });

            var result =
                sut.GetExampleRecords(null);

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Contains("product name", result.Header);
            Assert.Contains("price-in-EUR", result.Header);
            Assert.Equal("Chocolate", result.Items[0].Fields["product name"].Value);
            Assert.Equal("2.5", result.Items[0].Fields["price-in-EUR"].Value);
        }

        [Fact]
        public void Can_map_the_same_source_field_to_multiple_result_fields()
        {
            var sut = CreateSut(new Dictionary<string,string>
            {
                { "product name", "product" },
                { "product code", "product" },
            });

            var result =
                sut.GetExampleRecords(null);

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Contains("product name", result.Header);
            Assert.Contains("product code", result.Header);
            Assert.Equal("Chocolate", result.Items[0].Fields["product name"].Value);
            Assert.Equal("Chocolate", result.Items[0].Fields["product code"].Value);
        }

        [Fact]
        public void Should_list_all_fields_that_were_not_specified_explicitly()
        {
            var sut = CreateSut(new Dictionary<string,string>
            {
                { "product name", "product" },
            });

            var result =
                sut.GetExampleRecords(null);

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Contains("product name", result.Header);
            Assert.Contains("price", result.Header);
            Assert.Contains("color", result.Header);
            Assert.Equal("brown", result.Items[0].Fields["color"].Value);
            Assert.Equal("Chocolate", result.Items[0].Fields["product name"].Value);
            Assert.Equal("2.5", result.Items[0].Fields["price"].Value);
        }

        [Fact]
        public void Should_transform_underscores_to_spaces_when_required_headers_have_spaces()
        {
            var sut = CreateSut(new Dictionary<string,string>
            {
                { "product_name", "product" },
            });

            var result =
                sut.GetExampleRecords(new []{ "product name"});

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Contains("product name", result.Header);
            Assert.Equal("Chocolate", result.Items[0].Fields["product name"].Value);
        }
    }
}
