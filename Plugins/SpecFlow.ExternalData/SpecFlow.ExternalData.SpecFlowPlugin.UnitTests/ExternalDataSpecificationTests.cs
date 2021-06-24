using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;
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

        private ExternalDataSpecification CreateSut()
        {
            return new(new DataValue(CreateProductDataList()));
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
    }
}
