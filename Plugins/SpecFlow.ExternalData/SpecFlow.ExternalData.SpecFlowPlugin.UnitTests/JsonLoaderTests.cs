using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class JsonLoaderTests
    {
        private static readonly string SampleFilesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "SampleFiles");
        private string _productsSampleFilePath = Path.Combine(SampleFilesFolder, "products.json");
        private string _nestedProductsSampleFilePath = Path.Combine(SampleFilesFolder, "products-nested-dataset.json");

        private string SampleFeatureFilePathInSampleFileFolder =>
            Path.Combine(Path.GetDirectoryName(_productsSampleFilePath) ?? ".", "Sample.feature");

        private JsonLoader CreateSut() => new();

        [Fact]
        public void Can_read_simple_json_file()
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
        public void Can_handle_relative_path()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(
                Path.GetFileName(_productsSampleFilePath), 
                SampleFeatureFilePathInSampleFileFolder);

            Assert.True(result.IsDataRecord);
        }

        
        [Fact]
        public void Returns_name_of_first_object_array_as_default_data_set()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_productsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.Equal("products", result.DefaultDataSet);
        }

        [Fact]
        public void Can_read_products_from_json_with_nested_dataset()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_nestedProductsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.True(result.IsDataRecord);
            Assert.True(result.AsDataRecord.Fields.ContainsKey("products"));
            var worksheetResult = result.AsDataRecord.Fields["products"];
            Assert.True(worksheetResult.IsDataTable);
            Assert.Equal(3, worksheetResult.AsDataTable.Items.Count);
            var firstItem = worksheetResult.AsDataTable.Items[0];
            Assert.Equal(3, firstItem.Fields.Count);
            Assert.Equal("Chocolate", firstItem.Fields["product"].AsString());
            Assert.Equal("brown", firstItem.Fields["color"].AsString());
            Assert.Equal("[\r\n  {\r\n    \"name\": \"Dark Chocolate\",\r\n    \"price\": \"1.6\"\r\n  },\r\n  {\r\n    \"name\": \"Milk Chocolate\",\r\n    \"price\": \"1.55\"\r\n  }\r\n]", firstItem.Fields["varieties"].AsString());
        }

        [Fact]
        public void Reads_nested_dataset()
        {
            var sut = CreateSut();
            var result = sut.LoadDataSource(_nestedProductsSampleFilePath, null);

            Assert.NotNull(result);
            Assert.True(result.IsDataRecord);
            Assert.True(result.AsDataRecord.Fields.ContainsKey("products.varieties"));
            var worksheetResult = result.AsDataRecord.Fields["products.varieties"];
            Assert.True(worksheetResult.IsDataTable);
            Assert.Equal(6, worksheetResult.AsDataTable.Items.Count);
            var firstItem = worksheetResult.AsDataTable.Items.First();
            Assert.Equal(4, firstItem.Fields.Count);
            Assert.Equal("Chocolate", firstItem.Fields["product"].AsString());
            Assert.Equal("brown", firstItem.Fields["color"].AsString());
            Assert.Equal("1.6", firstItem.Fields["price"].AsString(CultureInfo.GetCultureInfo("en-us")));
            Assert.Equal("Dark Chocolate", firstItem.Fields["name"].AsString());
            
            var lastItem = worksheetResult.AsDataTable.Items.Last();
            Assert.Equal(4, lastItem.Fields.Count);
            Assert.Equal("Orange", lastItem.Fields["product"].AsString());
            Assert.Equal("orange", lastItem.Fields["color"].AsString());
            Assert.Equal("1.6", lastItem.Fields["price"].AsString(CultureInfo.GetCultureInfo("en-us")));
            Assert.Equal("Tangerine", lastItem.Fields["name"].AsString());
        }
    }
}
