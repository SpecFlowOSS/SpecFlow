using System;
using System.CodeDom;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGenerator
    {
        CodeNamespace GenerateUnitTestFixture(SpecFlowFeature feature, string testClassName, string targetNamespace);
    }
}