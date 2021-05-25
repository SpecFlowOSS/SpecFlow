using System;
using System.IO;
using System.Reflection;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class CsvLoaderTests
    {
        private static readonly string SampleFilesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "SampleFiles");
        private readonly string _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products.csv");

        private CsvLoader CreateSut() => new();

        [Fact]
        public void Can_read_simple_csv_file()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null, null);
            
            Assert.True(result.IsDataList);
            Assert.Equal(3, result.AsDataList.Items.Count);
            Assert.True(result.AsDataList.Items[0].IsDataRecord);
            Assert.Equal("Chocolate", result.AsDataList.Items[0].AsDataRecord.Fields["product"].AsString);
            Assert.Equal("2.5", result.AsDataList.Items[0].AsDataRecord.Fields["price"].AsString);
        }
    }
}
