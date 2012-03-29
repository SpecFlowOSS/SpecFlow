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
            get { return PriorityValues.Lowest; }
        }

        public bool CanGenerate(Feature feature)
        {
            return true;
        }

        public IFeatureGenerator CreateGenerator(Feature feature)
        {
            return unitTestFeatureGenerator;
        }
    }
}