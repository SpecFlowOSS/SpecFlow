using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class FeatureGeneratorRegistry : IFeatureGeneratorRegistry
    {
        private readonly List<KeyValuePair<string, IFeatureGeneratorProvider>> providers;

        public FeatureGeneratorRegistry(IObjectContainer objectContainer)
        {
            providers = objectContainer.Resolve<IDictionary<string, IFeatureGeneratorProvider>>().OrderBy(item => item.Value.Priority).ToList();
        }

        public IFeatureGenerator CreateGenerator(SpecFlowDocument document)
        {
            var providerItem = FindProvider(document);
            return providerItem.Value.CreateGenerator(document);
        }

        private KeyValuePair<string, IFeatureGeneratorProvider> FindProvider(SpecFlowDocument feature)
        {
            return providers.First(item => item.Value.CanGenerate(feature));
        }
    }
}