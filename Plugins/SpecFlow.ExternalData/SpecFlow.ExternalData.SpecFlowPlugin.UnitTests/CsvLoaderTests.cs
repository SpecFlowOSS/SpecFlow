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

        [Fact]
        public void Can_read_special_csv_file()
        {
            var sut = CreateSut();
            var result = sut.LoadCsvDataList(@"""product"",price
Chocolate,2.5
""One""""Two,Three "",1.0
""Orange
Juice"", 1.2", CultureInfo.CurrentCulture);

            Assert.Equal(3, result.Items.Count);
            Assert.True(result.Items[0].IsDataRecord);
            Assert.Equal("Chocolate", result.Items[0].AsDataRecord.Fields["product"].AsString);
            Assert.Equal("2.5", result.Items[0].AsDataRecord.Fields["price"].AsString);
            Assert.Equal(@"One""Two,Three ", result.Items[1].AsDataRecord.Fields["product"].AsString);
            Assert.Equal(@"Orange
Juice", result.Items[2].AsDataRecord.Fields["product"].AsString);
            Assert.Equal("1.2", result.Items[2].AsDataRecord.Fields["price"].AsString);
        }

        [Fact]
        public void Can_handle_relative_path()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Can_handle_invalid_path()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Can_handle_invalid_csv_file_format()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Can_handle_csv_without_header_row()
        {
            throw new NotImplementedException();
        }
    }
}
