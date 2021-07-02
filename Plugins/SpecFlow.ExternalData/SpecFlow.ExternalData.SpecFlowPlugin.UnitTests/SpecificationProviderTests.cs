using System;
using Gherkin.Ast;
using Moq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class SpecificationProviderTests
    {
        private const string SOURCE_FILE_PATH = @"C:\Temp\Sample.feature";
        private readonly Mock<IDataSourceLoaderFactory> _dataSourceLoaderFactoryMock = new();
        private readonly Mock<IDataSourceLoader> _dataSourceLoaderMock = new();

        public SpecificationProviderTests()
        {
            _dataSourceLoaderFactoryMock.Setup(f => f.CreateLoader(It.IsAny<string>(), It.IsAny<string>()))
                                        .Returns(_dataSourceLoaderMock.Object);
        }
        
        private SpecificationProvider CreateSut()
        {
            return new(_dataSourceLoaderFactoryMock.Object, new DataSourceSelectorParser());
        }

        [Fact]
        public void Should_return_null_if_no_data_source_tag()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[] { new Tag(null, @"@other-tag") }, SOURCE_FILE_PATH);
            
            Assert.Null(result);
        }

        [Theory]
        [InlineData("@DataSource:")]
        [InlineData("@DataSource")]
        public void Should_handle_invalid_data_source_tags(string tag)
        {
            var sut = CreateSut();

            Assert.Throws<ExternalDataPluginException>(() => sut.GetSpecification(new[]
            {
                new Tag(null, tag)
            }, SOURCE_FILE_PATH));
        }

        [Theory]
        [InlineData("@DataField")]
        [InlineData("@DataField:")]
        public void Should_handle_invalid_tags(string tag)
        {
            var sut = CreateSut();

            Assert.Throws<ExternalDataPluginException>(() => sut.GetSpecification(new[]
            {
                new Tag(null, "@DataSource:foo"),
                new Tag(null, tag)
            }, SOURCE_FILE_PATH));
        }

        [Fact]
        public void Should_get_data_source_path_from_tags()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv")
            }, SOURCE_FILE_PATH);
            
            Assert.NotNull(result);
            _dataSourceLoaderMock.Verify(l => l.LoadDataSource(@"path\to\file.csv", It.IsAny<string>()));
        }

        [Fact]
        public void Should_be_able_to_disable_data_source()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DisableDataSource")
            }, SOURCE_FILE_PATH);

            Assert.Null(result);
        }

        [Fact]
        public void Should_use_last_DataSource_setting_for_duplicated_tags()
        {
            var sut = CreateSut();

            sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file1.csv"),
                new Tag(null, @"@DataSource:path\to\file2.csv")
            }, SOURCE_FILE_PATH);

            _dataSourceLoaderMock.Verify(l => 
                l.LoadDataSource(@"path\to\file2.csv", It.IsAny<string>()));
        }

        [Fact]
        public void Should_pass_on_source_file_path()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[] { new Tag(null, @"@DataSource:path\to\file.csv") }, SOURCE_FILE_PATH);
            
            Assert.NotNull(result);
            _dataSourceLoaderMock.Verify(l => l.LoadDataSource(It.IsAny<string>(), SOURCE_FILE_PATH));
        }

        [Fact]
        public void Should_pass_source_file_to_loader_factory()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv")
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            _dataSourceLoaderFactoryMock.Verify(f => f.CreateLoader(It.IsAny<string>(), @"path\to\file.csv"));
        }

        [Fact]
        public void Should_get_data_format_from_tags()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DataFormat:csv")
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            _dataSourceLoaderFactoryMock.Verify(f => f.CreateLoader("csv", It.IsAny<string>()));
        }

        [Fact]
        public void Should_use_last_DataFormat_setting_for_duplicated_tags()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DataFormat:xlsx"),
                new Tag(null, @"@DataFormat:csv")
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            _dataSourceLoaderFactoryMock.Verify(f => f.CreateLoader("csv", It.IsAny<string>()));
        }



        [Fact]
        public void Should_collect_field_mappings_from_tags()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DataField:target_field=source_field"),
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            Assert.NotNull(result.SpecifiedFieldSelectors);
            Assert.Contains("target_field", result.SpecifiedFieldSelectors.Keys);
            Assert.Equal("source_field", result.SpecifiedFieldSelectors["target_field"].ToString());
        }

        [Fact]
        public void Should_use_last_DataField_setting_for_duplicated_target_fields()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DataField:target_field=source_field1"),
                new Tag(null, @"@DataField:target_field=source_field2"),
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            Assert.NotNull(result.SpecifiedFieldSelectors);
            Assert.Contains("target_field", result.SpecifiedFieldSelectors.Keys);
            Assert.Equal("source_field2", result.SpecifiedFieldSelectors["target_field"].ToString());
        }

        [Theory]
        [InlineData("@DataField:target_field")]
        [InlineData("@DataField:target_field=")]
        public void Should_use_target_field_as_source_when_DataField_setting_does_not_contain_value(string dataFieldTag)
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, dataFieldTag)
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            Assert.NotNull(result.SpecifiedFieldSelectors);
            Assert.Contains("target_field", result.SpecifiedFieldSelectors.Keys);
            Assert.Equal("target_field", result.SpecifiedFieldSelectors["target_field"].ToString());
        }

        [Fact]
        public void Should_collect_data_sets_from_tags()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DataSet:data-set-name"),
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            Assert.NotNull(result.DataSet);
            Assert.Equal("data-set-name", result.DataSet);
        }

        [Fact]
        public void Should_use_last_DataSet_setting_for_duplicated_target_fields()
        {
            var sut = CreateSut();

            var result = sut.GetSpecification(new[]
            {
                new Tag(null, @"@DataSource:path\to\file.csv"),
                new Tag(null, @"@DataSet:data-set-name1"),
                new Tag(null, @"@DataSet:data-set-name2"),
            }, SOURCE_FILE_PATH);

            Assert.NotNull(result);
            Assert.NotNull(result.DataSet);
            Assert.Equal("data-set-name2", result.DataSet);
        }

    }
}
