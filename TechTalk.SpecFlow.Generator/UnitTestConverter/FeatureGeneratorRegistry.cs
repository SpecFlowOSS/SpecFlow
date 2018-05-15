using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class FeatureGeneratorRegistry : IFeatureGeneratorRegistry
    {
        private readonly List<IFeatureGeneratorProvider> providers;

        public FeatureGeneratorRegistry(IObjectContainer objectContainer)
        {
            providers = objectContainer.ResolveAll<IFeatureGeneratorProvider>().ToList().OrderBy(item => item.Priority).ToList();
        }

        public IFeatureGenerator CreateGenerator(SpecFlowDocument document)
        {
            var providerItem = FindProvider(document);
            return providerItem.CreateGenerator(document);
        }

        private IFeatureGeneratorProvider FindProvider(SpecFlowDocument feature)
        {
            return providers.First(item => item.CanGenerate(feature));
        }
    }
}