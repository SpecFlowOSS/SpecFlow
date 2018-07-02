using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]


namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            //unitTestProviderConfiguration.UseUnitTestProvider("specs-multiple-configurations");
            generatorPluginEvents.RegisterDependencies += GeneratorPluginEvents_RegisterDependencies;
            generatorPluginEvents.CustomizeDependencies += GeneratorPluginEvents_CustomizeDependencies;
        }

        private void GeneratorPluginEvents_CustomizeDependencies(object sender, CustomizeDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<MultiFeatureGeneratorProvider, IFeatureGeneratorProvider>("specs-multiple-configurations");
        }

        private void GeneratorPluginEvents_RegisterDependencies(object sender, RegisterDependenciesEventArgs e)
        {
        }
    }
}
