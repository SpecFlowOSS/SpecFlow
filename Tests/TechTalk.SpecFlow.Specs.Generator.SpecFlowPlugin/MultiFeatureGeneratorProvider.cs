using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    internal class MultiFeatureGeneratorProvider : IFeatureGeneratorProvider
    {
        

        private readonly MultiFeatureGenerator _multiFeatureGenerator;

        public MultiFeatureGeneratorProvider(IObjectContainer container)
        {
            var featureGenerators = new List<KeyValuePair<Combination, IFeatureGenerator>>();

            foreach (var combination in TestRunCombinations.List)
            {
                var combinationFeatureGenerator = new CombinationFeatureGenerator(container.Resolve<CodeDomHelper>(), container.Resolve<SpecFlowConfiguration>(), container.Resolve<IDecoratorRegistry>(), combination, container.Resolve<ProjectSettings>());
                featureGenerators.Add(new KeyValuePair<Combination, IFeatureGenerator>(combination, combinationFeatureGenerator));
            }

            _multiFeatureGenerator = new MultiFeatureGenerator(featureGenerators, new CombinationFeatureGenerator(container.Resolve<CodeDomHelper>(), container.Resolve<SpecFlowConfiguration>(), container.Resolve<IDecoratorRegistry>(), null, container.Resolve<ProjectSettings>()));
        }


        public bool CanGenerate(SpecFlowDocument specFlowDocument)
        {
            return true;
        }

        public IFeatureGenerator CreateGenerator(SpecFlowDocument specFlowDocument)
        {
            return _multiFeatureGenerator;
        }

        public int Priority => PriorityValues.Normal;
    }
}