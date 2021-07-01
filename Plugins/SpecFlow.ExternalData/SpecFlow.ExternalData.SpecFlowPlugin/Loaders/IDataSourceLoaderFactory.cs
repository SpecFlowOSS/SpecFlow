namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public interface IDataSourceLoaderFactory
    {
        IDataSourceLoader CreateLoader();
    }
}
