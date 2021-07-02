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
        public void Should_always_create_FieldNameSelector_for_now()
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
            Assert.Equal("value2", result.AsString);
        }
    }
}
