using TechTalk.SpecFlow.Parser;

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

        public bool CanGenerate(SpecFlowFeature feature)
        {
            return true;
        }

        public IFeatureGenerator CreateGenerator(SpecFlowFeature feature)
        {
            return unitTestFeatureGenerator;
        }
    }
}