using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class CsvLoaderTests
    {
        private static readonly string SampleFilesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "SampleFiles");
        private string _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products.csv");

        private string SampleFeatureFilePathInSampleFileFolder =>
            Path.Combine(Path.GetDirectoryName(_productsSampleFilePath) ?? ".", "Sample.feature");

        private CsvLoader CreateSut() => new();

        [Fact]
        public void Can_read_simple_csv_file()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);
            
            Assert.True(result.IsDataTable);
            Assert.Equal(3, result.AsDataTable.Items.Count);
            Assert.Equal("Chocolate", result.AsDataTable.Items[0].Fields["product"].AsString());
            Assert.Equal("2.5", result.AsDataTable.Items[0].Fields["price"].AsString());
        }

        [Fact]
        public void Can_read_special_csv_file()
        {
            var sut = CreateSut();
            var resultDataSource = sut.LoadDataSource(
                "products-special.csv",
                SampleFeatureFilePathInSampleFileFolder);

            var result = resultDataSource.AsDataTable;
            Assert.Equal(new[] {"product", "price"}, result.Header);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal("Chocolate", result.Items[0].Fields["product"].AsString());
            Assert.Equal("2.5", result.Items[0].Fields["price"].AsString());
            Assert.Equal(@"One""Two,Three ", result.Items[1].Fields["product"].AsString());
            Assert.Equal(@"Orange
Juice", result.Items[2].Fields["product"].AsString());
            Assert.Equal("1.2", result.Items[2].Fields["price"].AsString());
        }

        [Fact]
        public void Can_handle_relative_path()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(
                Path.GetFileName(_productsSampleFilePath), 
                SampleFeatureFilePathInSampleFileFolder);

            Assert.True(result.IsDataTable);
        }

        [Fact]
        public void Can_handle_invalid_path()
        {
            var sut = CreateSut();
            
            Assert.Throws<ExternalDataPluginException>(() => 
                sut.LoadDataSource(
                    "no-such-file.csv",
                    SampleFeatureFilePathInSampleFileFolder));
        }

        [Fact]
        public void Can_handle_invalid_csv_file_format()
        {
            _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products-invalid.csv");

            var sut = CreateSut();
            Assert.Throws<ExternalDataPluginException>(() => 
                sut.LoadDataSource(_productsSampleFilePath, null));
        }

        [Fact]
        public void Can_handle_empty_csv_with_header_only()
        {
            _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products-empty.csv");

            var sut = CreateSut();
            
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.True(result.IsDataTable);
            Assert.Equal(0, result.AsDataTable.Items.Count);
        }
    }
}
