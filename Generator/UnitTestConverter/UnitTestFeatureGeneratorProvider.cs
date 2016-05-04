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

        public bool CanGenerate(SpecFlowDocument document)
        {
            return true;
        }

        public IFeatureGenerator CreateGenerator(SpecFlowDocument document)
        {
            return unitTestFeatureGenerator;
        }
    }
}