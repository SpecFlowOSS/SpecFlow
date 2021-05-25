using System;
using System.Collections.Generic;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSource
{
    public class DataRecord
    {
        public IDictionary<string, DataValue> Fields { get; } = new Dictionary<string, DataValue>();
    }
}
