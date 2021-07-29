using System;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public interface IDataSourceLoader
    {
        bool AcceptsSourceFilePath(string sourceFilePath);
        DataSource LoadDataSource(string path, string sourceFilePath);
    }
}
