using System;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public interface IDataSourceLoader
    {
        bool AcceptsSourceFilePath(string sourceFilePath);
        DataValue LoadDataSource(string path, string sourceFilePath);
    }
}
