using System;
namespace TechTalk.SpecFlow.Generator
{
	interface ISpecFlowUnitTestConverter
	{
		System.CodeDom.CodeNamespace GenerateUnitTestFixture(TechTalk.SpecFlow.Parser.SyntaxElements.Feature feature, string testClassName, string targetNamespace);
	}
}
