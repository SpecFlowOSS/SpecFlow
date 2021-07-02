namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors
{
    public class FieldNameSelector : DataSourceSelector
    {
        private readonly string _name;

        public FieldNameSelector(string name)
        {
            _name = name;
        }

        public override DataValue Evaluate(DataValue dataValue)
        {
            if (!dataValue.IsDataRecord)
                throw new ExternalDataPluginException($"The expression '{_name}' cannot be evaluated on value '{dataValue.Value}', because it is not a record type but an '{dataValue.Value?.GetType().Name ?? "null"}'");

            return dataValue.AsDataRecord.Fields[_name];
        }
    }
}
