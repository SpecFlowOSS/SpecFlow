using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSource
{
    public class ExternalDataSpecification
    {
        public DataValue DataSource { get; }

        public ExternalDataSpecification(DataValue dataSource)
        {
            DataSource = dataSource;
        }

        public IEnumerable<DataRecord> GetExampleRecords()
        {
            //TODO: handle different data sources
            //TODO: handle data sets
            //TODO: handle data fields
            Debug.Assert(DataSource.IsDataList);
            var dataList = DataSource.AsDataList;
            foreach (var dataListItem in dataList.Items)
            {
                Debug.Assert(dataListItem.IsDataRecord);
                yield return dataListItem.AsDataRecord;
            }
        }
    }
}
