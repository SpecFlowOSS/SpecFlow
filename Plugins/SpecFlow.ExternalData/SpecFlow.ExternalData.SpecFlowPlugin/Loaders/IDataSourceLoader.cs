using System;
using System.Globalization;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public interface IDataSourceLoader
    {
        DataValue LoadDataSource(string path, string sourceFilePath);
    }
}
