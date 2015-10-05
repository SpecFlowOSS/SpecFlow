using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class NUnit3TestGeneratorProvider : NUnitTestGeneratorProvider
    {
        public NUnit3TestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.RowTests | UnitTestGeneratorTraits.ParallelExecution;
        }
    }
}