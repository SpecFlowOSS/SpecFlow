using TechTalk.SpecFlow.Parser.SyntaxElements;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGeneratorRegistry
    {
        IFeatureGenerator CreateGenerator(Feature feature);
    }
}