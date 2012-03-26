using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class UnitTestFeatureGeneratorProvider : IFeatureGeneratorProvider
    {
        private readonly UnitTestFeatureGenerator unitTestFeatureGenerator;

        public UnitTestFeatureGeneratorProvider(UnitTestFeatureGenerator unitTestFeatureGenerator)
        {
            this.unitTestFeatureGenerator = unitTestFeatureGenerator;
        }

        public int Priority
        {
            get { return int.MaxValue; }
        }

        public bool CanGenerate(Feature feature, string registeredName)
        {
            return true;
        }

        public IFeatureGenerator CreateGenerator(Feature feature, string registeredName)
        {
            return unitTestFeatureGenerator;
        }
    }
}