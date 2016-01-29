using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGeneratorRegistry
    {
        IFeatureGenerator CreateGenerator(SpecFlowFeature feature);
    }
}