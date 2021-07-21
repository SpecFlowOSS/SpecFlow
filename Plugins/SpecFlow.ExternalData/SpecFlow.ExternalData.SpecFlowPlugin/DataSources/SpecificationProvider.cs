using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources
{
    public class SpecificationProvider : ISpecificationProvider
    {
        private const string DATA_SOURCE_TAG_PREFIX = "@DataSource";
        private const string DISABLE_DATA_SOURCE_TAG = "@DisableDataSource";
        private const string DATA_FIELD_TAG_PREFIX = "@DataField";
        private const string DATA_FORMAT_TAG_PREFIX = "@DataFormat";
        private const string DATA_SET_TAG_PREFIX = "@DataSet";

        private readonly IDataSourceLoaderFactory _dataSourceLoaderFactory;
        private readonly DataSourceSelectorParser _dataSourceSelectorParser;
        private readonly StringComparison _tagNameComparison = StringComparison.CurrentCulture;

        public SpecificationProvider(IDataSourceLoaderFactory dataSourceLoaderFactory, DataSourceSelectorParser dataSourceSelectorParser)
        {
            _dataSourceLoaderFactory = dataSourceLoaderFactory;
            _dataSourceSelectorParser = dataSourceSelectorParser;
        }

        public ExternalDataSpecification GetSpecification(IEnumerable<Tag> tags, string sourceFilePath)
        {
            var tagsArray = tags.ToArray();
            var dataSourcePath = GetTagValues(tagsArray, DATA_SOURCE_TAG_PREFIX)
                .LastOrDefault();
            if (dataSourcePath == null || tagsArray.Any(t => t.Name.Equals(DISABLE_DATA_SOURCE_TAG, _tagNameComparison)))
                return null;

            var dataFormat = GetDataFormat(tagsArray);
            var dataSet = GetDataSet(tagsArray);
            
            var loader = _dataSourceLoaderFactory.CreateLoader(dataFormat, dataSourcePath);
            var dataSource = loader.LoadDataSource(dataSourcePath, sourceFilePath);
            var fields = GetFields(tagsArray);

            return new ExternalDataSpecification(dataSource, fields, dataSet);
        }

        private string GetDataSet(Tag[] tags)
        {
            return GetTagValues(tags, DATA_SET_TAG_PREFIX)
                .LastOrDefault();
        }

        private string GetDataFormat(Tag[] tags)
        {
            return GetTagValues(tags, DATA_FORMAT_TAG_PREFIX)
                .LastOrDefault();
        }

        private Dictionary<string, DataSourceSelector> GetFields(Tag[] tags)
        {
            var dataFieldsSettings = GetSettingValues(GetTagValues(tags, DATA_FIELD_TAG_PREFIX))
                                     .GroupBy(s => s.Key, s => s.Value)
                                     .ToArray();
            if (dataFieldsSettings.Length == 0) return null;
            return dataFieldsSettings
                .ToDictionary(
                    fs => fs.Key, 
                    fs =>
                    {
                        string value = fs.Last();
                        return _dataSourceSelectorParser.Parse(
                            string.IsNullOrEmpty(value) ? fs.Key : value); 
                    });
        }

        private IEnumerable<KeyValuePair<string, string>> GetSettingValues(IEnumerable<string> values)
        {
            return values.Select(
                v =>
                {
                    var parts = v.Split(new[] { '=' }, 2);
                    return new KeyValuePair<string, string>(parts[0], parts.Length == 2 ? parts[1] : null);
                });
        }
        
        private IEnumerable<string> GetTagValues(Tag[] tags, string prefix)
        {
            var prefixWithColon = prefix + ":";
            foreach (var tag in tags)
            {
                if (tag.Name.Equals(prefix, _tagNameComparison) || tag.Name.Equals(prefixWithColon, _tagNameComparison)) 
                    throw new ExternalDataPluginException($"Invalid tag '{tag.Name}'. The tag should have a value in '{prefix}:value' form.");
                
                if (tag.Name.StartsWith(prefixWithColon, _tagNameComparison)) 
                    yield return tag.Name.Substring(prefixWithColon.Length);
            }
        }
    }
}
