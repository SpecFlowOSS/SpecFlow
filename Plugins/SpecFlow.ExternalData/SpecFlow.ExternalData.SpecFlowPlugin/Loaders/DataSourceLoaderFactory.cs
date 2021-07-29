using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public class DataSourceLoaderFactory : IDataSourceLoaderFactory
    {
        private readonly IDictionary<string, IDataSourceLoader> _dataSourceLoaders;

        public DataSourceLoaderFactory(IDictionary<string, IDataSourceLoader> dataSourceLoaders)
        {
            _dataSourceLoaders = new Dictionary<string, IDataSourceLoader>(dataSourceLoaders, StringComparer.InvariantCultureIgnoreCase);
        }
        
        public IDataSourceLoader CreateLoader(string format, string dataSourcePath)
        {
            if (format != null)
            {
                if (_dataSourceLoaders.TryGetValue(format, out var result)) 
                    return result;
                throw new ExternalDataPluginException($"Unable to find external data loader for format '{format}'. Supported formats: {string.Join(", ", _dataSourceLoaders.Keys)}");
            }

            var loader = _dataSourceLoaders.Values
                .FirstOrDefault(l => l.AcceptsSourceFilePath(dataSourcePath));
            
            if (loader == null)
                throw new ExternalDataPluginException($"Unable to find external data loader for path '{dataSourcePath}'. Supported formats: {string.Join(", ", _dataSourceLoaders.Keys)}");

            return loader;
        }
    }
}
