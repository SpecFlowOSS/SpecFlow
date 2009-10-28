using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.VsIntegration
{
    public class SpecFlowTestGenerator
    {
        static public CodeCompileUnit GenerateTestFile(string content, string featureFileName, string targetNamespace)
        {
            SpecFlowLangParser parser = new SpecFlowLangParser();
            Feature feature = parser.Parse(new StringReader(content), featureFileName);

            SpecFlowUnitTestConverter testConverter = new SpecFlowUnitTestConverter();
            CodeCompileUnit codeCompileUnit = testConverter.GenerateUnitTestFixture(feature, null, targetNamespace);

            return codeCompileUnit;
        }
    }
}
