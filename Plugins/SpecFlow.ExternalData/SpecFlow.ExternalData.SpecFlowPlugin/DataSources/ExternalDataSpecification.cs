using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources
{
    public class ExternalDataSpecification
    {
        public DataValue DataSource { get; }
        public IDictionary<string, string> Fields { get; }

        public ExternalDataSpecification(DataValue dataSource, IDictionary<string, string> fields = null)
        {
            Fields = fields == null ? null : new ReadOnlyDictionary<string, string>(fields);
            DataSource = dataSource;
        }

        public DataTable GetExampleRecords(string[] examplesHeaderNames)
        {
            //TODO: handle different data sources
            //TODO: handle data sets
            if (!DataSource.IsDataTable) throw new NotImplementedException();
            
            var dataTable = DataSource.AsDataTable;

            var headerNames = examplesHeaderNames ?? GetTargetHeader(dataTable.Header);

            var result = new DataTable(headerNames);
            foreach (var dataRecord in dataTable.Items)
            {
                result.Items.Add(new DataRecord(
                    headerNames.Select(targetField => 
                        new KeyValuePair<string, DataValue>(
                            targetField, 
                            GetSourceFieldValue(dataRecord, targetField, dataTable.Header)))));
            }
            return result;
        }

        private DataValue GetSourceFieldValue(DataRecord dataRecord, string targetField, string[] dataTableHeader)
        {
            var sourceField = GetSourceField(targetField);
            return dataRecord.Fields.TryGetValue(sourceField, out var value) ||
                   dataRecord.Fields.TryGetValue(GetSourceField(targetField.Replace(' ', '_')), out value)
                ? value
                : throw new ExternalDataPluginException($"The requested field '{sourceField}' was not provided in the data source, that provides only the fields {string.Join(", ", GetTargetHeader(dataTableHeader).Select(f => "'" + f + "'"))}. Try mapping source fields using '@DataField:target-field=source-field'.");
        }

        private string GetSourceField(string targetField)
        {
            if (Fields == null) return targetField;
            return Fields.TryGetValue(targetField, out var sourceField) ?
                sourceField : targetField;
        }
        
        private string[] GetTargetHeader(string[] dataTableHeader)
        {
            if (Fields == null) return dataTableHeader;
            return Fields.Keys.Concat(dataTableHeader.Where(sourceField => !Fields.Any(f => FieldEquals(sourceField, f.Value)))).ToArray();
        }

        private bool FieldEquals(string sourceField, string targetField) => sourceField.Equals(targetField);
    }
}
