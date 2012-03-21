using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class FeatureGeneratorRegistry : IFeatureGeneratorRegistry
    {
        private readonly List<KeyValuePair<string, IFeatureGeneratorProvider>> providers;

        public FeatureGeneratorRegistry(IObjectContainer objectContainer)
        {
            providers = objectContainer.Resolve<IDictionary<string, IFeatureGeneratorProvider>>().OrderBy(item => item.Value.Priority).ToList();
        }

        public IFeatureGenerator CreateGenerator(Feature feature)
        {
            var featureGeneratorProvider = FindProvider(feature);
            return featureGeneratorProvider.CreateGenerator(feature);
        }

        private IFeatureGeneratorProvider FindProvider(Feature feature)
        {
            return providers.First(item => item.Value.CanGenerate(feature)).Value;
        }
    }
}