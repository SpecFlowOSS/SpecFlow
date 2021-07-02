using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources
{
    public class ExternalDataSpecification
    {
        public DataSource DataSource { get; }
        public string DataSet { get; }
        public IDictionary<string, DataSourceSelector> SpecifiedFieldSelectors { get; }

        public ExternalDataSpecification(DataSource dataSource, IDictionary<string, DataSourceSelector> specifiedFieldSelectors = null, string dataSet = null)
        {
            SpecifiedFieldSelectors = specifiedFieldSelectors == null ? null : 
                new ReadOnlyDictionary<string, DataSourceSelector>(
                    new Dictionary<string, DataSourceSelector>(specifiedFieldSelectors, FieldNameComparer.Value));
            DataSource = dataSource;
            DataSet = dataSet;
        }

        public DataTable GetExampleRecords(string[] examplesHeaderNames)
        {
            DataValue dataSet = DataSource;
            if (DataSource.IsDataRecord)
            {
                if (DataSet != null)
                    dataSet = DataSource.AsDataRecord.Fields.TryGetValue(DataSet, out var value)?
                            value:
                            throw new ExternalDataPluginException($"Unable to find data set '{DataSet}' in the data source. Available data sets: {string.Join(", ", DataSource.AsDataRecord.Fields.Keys)}");
                else if (DataSource.DefaultDataSet != null)
                    dataSet = DataSource.AsDataRecord.Fields[DataSource.DefaultDataSet];
            }
            
            //TODO: handle hierarchical data
            if (!dataSet.IsDataTable)
                throw new ExternalDataPluginException("Hierarchical data sources are not supported yet.");

            var dataTable = dataSet.AsDataTable;

            var headerNames = examplesHeaderNames ?? GetTargetHeader(dataTable.Header);
            var fieldSelectors = GetFieldSelectors(headerNames);

            var result = new DataTable(headerNames);
            foreach (var dataRecord in dataTable.Items)
            {
                result.Items.Add(new DataRecord(
                    fieldSelectors.Select(targetFieldSelector => 
                        new KeyValuePair<string, DataValue>(
                            targetFieldSelector.Key, 
                            targetFieldSelector.Value.Evaluate(new DataValue(dataRecord))))));
            }
            return result;
        }

        private Dictionary<string, DataSourceSelector> GetFieldSelectors(string[] headerNames)
        {
            return headerNames.ToDictionary(h => h, GetSourceFieldSelector);
        }

        private DataSourceSelector GetSourceFieldSelector(string targetField)
        {
            if (SpecifiedFieldSelectors != null && 
                SpecifiedFieldSelectors.TryGetValue(targetField, out var selector)) 
                return selector;
            return new FieldNameSelector(targetField);
        }

        private string[] GetTargetHeader(string[] dataTableHeader)
        {
            if (SpecifiedFieldSelectors == null) return dataTableHeader;
            return SpecifiedFieldSelectors.Keys.Concat(dataTableHeader.Where(sourceField => !SpecifiedFieldSelectors.Values.Any(selector => FieldUsedBy(sourceField, selector)))).ToArray();
        }

        private bool FieldUsedBy(string field, DataSourceSelector selector) 
            => selector is FieldNameSelector fieldNameSelector && fieldNameSelector.Name.Equals(field);
    }
}
