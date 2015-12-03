using System;
using System.Linq;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGeneratorProvider
    {
        int Priority { get; }
        bool CanGenerate(SpecFlowFeature feature);
        IFeatureGenerator CreateGenerator(SpecFlowFeature feature);
    }
}