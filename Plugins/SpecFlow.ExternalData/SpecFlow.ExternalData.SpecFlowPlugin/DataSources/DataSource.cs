using System;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources
{
    public class DataSource : DataValue
    {
        public string DefaultDataSet { get; }

        public DataSource(object value, string defaultDataSet = null) : base(value)
        {
            DefaultDataSet = defaultDataSet;
        }
    }
}
