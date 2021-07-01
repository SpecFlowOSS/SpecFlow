namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public class DataSourceLoaderFactory : IDataSourceLoaderFactory
    {
        public IDataSourceLoader CreateLoader()
        {
            //TODO: decide on loader
            return new CsvLoader();
        }
    }
}
