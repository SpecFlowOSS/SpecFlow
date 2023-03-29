using System.Linq;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors
{
    public class FieldNameSelector : DataSourceSelector
    {
        public string Name { get; }

        public FieldNameSelector(string name)
        {
            Name = name;
        }

        public override DataValue EvaluateInternal(DataValue dataValue, bool throwException)
        {
            if (!dataValue.IsDataRecord)
                throw new ExternalDataPluginException($"The expression '{Name}' cannot be evaluated on value '{dataValue.Value}', because it is not a record type but an '{dataValue.Value?.GetType().Name ?? "null"}'");

            var dataRecord = dataValue.AsDataRecord;
            return dataRecord.Fields.TryGetValue(Name, out var value)
                ? value
                : throwException 
                    ? throw new ExternalDataPluginException($"The requested field '{Name}' was not provided in the data source, that provides only the fields {string.Join(", ", dataRecord.Fields.Keys.Select(f => "'" + f + "'"))}. Try mapping source fields using '@DataField:target-field=source-field'.") 
                    : null;
        }

        public override string ToString() => Name;
    }
}
