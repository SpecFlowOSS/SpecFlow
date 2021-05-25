using System;
using System.Collections.Generic;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSource
{
    public class DataList
    {
        public IList<DataValue> Items { get; } = new List<DataValue>();
    }
}
