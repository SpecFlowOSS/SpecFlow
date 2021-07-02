using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class ExcelLoaderTests
    {
        private static readonly string SampleFilesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "SampleFiles");
        private readonly string _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products.xlsx");

        private string SampleFeatureFilePathInSampleFileFolder =>
            Path.Combine(Path.GetDirectoryName(_productsSampleFilePath) ?? ".", "Sample.feature");

        private ExcelLoader CreateSut() => new();

        [Fact]
        public void Can_read_simple_Excel_file()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.True(result.IsDataTable);
            Assert.Equal(3, result.AsDataTable.Items.Count);
            Assert.Equal("Chocolate", result.AsDataTable.Items[0].Fields["product"].AsString);
            Assert.Equal("2.5", result.AsDataTable.Items[0].Fields["price"].AsString);
            Assert.Equal("brown", result.AsDataTable.Items[0].Fields["color"].AsString);
        }

    }
}
