using System;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests.DataSources.Selectors
{
    public class DataSourceSelectorParserTests
    {
        private DataSourceSelectorParser CreateSut() => new();

        [Fact]
        public void Should_always_create_FieldNameSelector_for_now()
        {
            // this is going to change when we support hierarchical data sets, like JSON
            var sut = CreateSut();

            var result = sut.Parse("some-value");
            
            Assert.NotNull(result);
            Assert.IsType<FieldNameSelector>(result);
        }
    }
}
