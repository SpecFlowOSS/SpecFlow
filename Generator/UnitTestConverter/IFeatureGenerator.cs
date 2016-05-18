using System;
using System.CodeDom;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IFeatureGenerator
    {
        CodeNamespace GenerateUnitTestFixture(SpecFlowDocument document, string testClassName, string targetNamespace);
    }
}