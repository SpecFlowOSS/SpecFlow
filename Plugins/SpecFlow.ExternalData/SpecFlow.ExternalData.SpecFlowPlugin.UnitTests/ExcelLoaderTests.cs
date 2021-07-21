using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class ExcelLoaderTests
    {
        private static readonly string SampleFilesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "SampleFiles");
        private readonly string _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products.xlsx");

        private ExcelLoader CreateSut() => new();

        [Fact]
        public void Can_read_simple_Excel_file()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.True(result.IsDataRecord);
            Assert.True(result.AsDataRecord.Fields.ContainsKey("products"));
            var worksheetResult = result.AsDataRecord.Fields["products"];
            Assert.True(worksheetResult.IsDataTable);
            Assert.Equal(3, worksheetResult.AsDataTable.Items.Count);
            Assert.Equal("Chocolate", worksheetResult.AsDataTable.Items[0].Fields["product"].AsString());
            Assert.Equal("2.5", worksheetResult.AsDataTable.Items[0].Fields["price"].AsString(CultureInfo.GetCultureInfo("en-us")));
            Assert.Equal("brown", worksheetResult.AsDataTable.Items[0].Fields["color"].AsString());
        }

        [Fact]
        public void Can_read_data_from_a_plain_worksheet()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.True(result.IsDataRecord);
            Assert.True(result.AsDataRecord.Fields.ContainsKey("products-plain"));
            var worksheetResult = result.AsDataRecord.Fields["products-plain"];
            Assert.True(worksheetResult.IsDataTable);
            Assert.Equal(3, worksheetResult.AsDataTable.Items.Count);
            Assert.Equal("Chocolate", worksheetResult.AsDataTable.Items[0].Fields["product"].AsString());
            Assert.Equal("2.5", worksheetResult.AsDataTable.Items[0].Fields["price"].AsString(CultureInfo.GetCultureInfo("en-us")));
            Assert.Equal("brown", worksheetResult.AsDataTable.Items[0].Fields["color"].AsString());
        }

        [Fact]
        public void Returns_name_of_first_sheet_as_default_data_set()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.Equal("products", result.DefaultDataSet);
        }

        [Fact]
        public void Reads_all_worksheets()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.True(result.IsDataRecord);
            Assert.True(result.AsDataRecord.Fields.ContainsKey("products"));
            Assert.True(result.AsDataRecord.Fields.ContainsKey("products-plain"));
        }

    }
}
