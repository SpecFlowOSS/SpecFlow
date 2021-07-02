using System;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors
{
    public abstract class DataSourceSelector
    {
        public abstract DataValue Evaluate(DataValue dataValue);
    }
}
