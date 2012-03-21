using System;
using BoDi;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGeneratorProvider
    {
        int Priority { get; }
        bool CanGenerate(Feature feature, string registeredName);
        IFeatureGenerator CreateGenerator(Feature feature);
    }
}