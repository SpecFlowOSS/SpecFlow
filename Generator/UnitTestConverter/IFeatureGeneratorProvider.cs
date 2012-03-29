using System;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGeneratorProvider
    {
        int Priority { get; }
        bool CanGenerate(Feature feature);
        IFeatureGenerator CreateGenerator(Feature feature);
    }
}