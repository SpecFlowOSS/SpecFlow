using SpecFlow.ExternalData.SpecFlowPlugin;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly:GeneratorPlugin(typeof(ExternalDataGeneratorPlugin))]

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public class ExternalDataGeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            generatorPluginEvents.RegisterDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<ExternalDataTestGenerator, ITestGenerator>();
                
                args.ObjectContainer.RegisterTypeAs<SpecificationProvider, ISpecificationProvider>();
                args.ObjectContainer.RegisterTypeAs<DataSourceLoaderFactory, IDataSourceLoaderFactory>();
                args.ObjectContainer.RegisterTypeAs<CsvLoader, IDataSourceLoader>("CSV");
                args.ObjectContainer.RegisterTypeAs<ExcelLoader, IDataSourceLoader>("Excel");
            };
        }
    }
}
