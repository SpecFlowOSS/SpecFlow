using System;
using System.Globalization;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources
{
    public class DataValue
    {
        public object Value { get; }

        public TValue As<TValue>() => (TValue)Value;
        public bool IsNull => Value == null;
        public bool IsDataList => Value is DataList;
        public DataList AsDataList => (DataList)Value;
        public bool IsDataTable => Value is DataTable;
        public DataTable AsDataTable => (DataTable)Value;
        public bool IsDataRecord => Value is DataRecord;
        public DataRecord AsDataRecord => (DataRecord)Value;

        public DataValue(object value)
        {
            Value = value;
        }

        public string AsString(CultureInfo cultureInfo = null)
        {
            if (Value == null || cultureInfo == null)
                return Value?.ToString();
            return Convert.ToString(Value, cultureInfo);
        }
    }
}
