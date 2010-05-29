using System;
using System.CodeDom;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface ISpecFlowUnitTestConverter
    {
        CodeNamespace GenerateUnitTestFixture(Feature feature, string testClassName, string targetNamespace);
    }
}