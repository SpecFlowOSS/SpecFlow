using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace ParserTests
{
    [TestFixture]
    public class SuccessfulMbUnitGenerationTest
    {
        private void CompareWithExpectedResult(Feature feature, string expectedResultFileName)
        {
            string expected = TestFileHelper.ReadFile(expectedResultFileName);
            string got = GenerateCodeFromFeature(feature);

            Assert.AreEqual(expected, got);
        }

        private void GenerateCodeFromFeature(Feature feature, TextWriter writer)
        {
            var mbUnitTestGeneratorProvider = new MbUnitTestGeneratorProvider();
            var converter = FactoryMethods.CreateUnitTestConverter(mbUnitTestGeneratorProvider);
            CodeNamespace codeNamespace = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

            var codeProvider = new CSharpCodeProvider();
            var options = new CodeGeneratorOptions();
            codeProvider.GenerateCodeFromNamespace(codeNamespace, writer, options);
        }

        private void GenerateCodeFromFeature(Feature feature, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                GenerateCodeFromFeature(feature, writer);
            }
        }

        private string GenerateCodeFromFeature(Feature feature)
        {
            using (var writer = new Utf8StringWriter())
            {
                GenerateCodeFromFeature(feature, writer);
                return writer.ToString();
            }
        }

        [Test, TestCaseSource(typeof (TestFileHelper), "GetTestFiles")]
        public void CanGenerateFromFile(string fileName)
        {
            Console.WriteLine(fileName);
            var parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StreamReader(fileName))
            {
                Feature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                string generatedCode = GenerateCodeFromFeature(feature);
                Assert.IsNotNull(generatedCode);

                // to regenerate the expected result file:
                //GenerateCodeFromFeature(feature, fileName + ".cs");

                //CompareWithExpectedResult(feature, fileName + ".cs");
            }
        }
    }
}