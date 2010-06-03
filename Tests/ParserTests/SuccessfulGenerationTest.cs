using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace ParserTests
{
    [TestFixture]
    public class SuccessfulGenerationTest
    {
        private void CompareWithExpectedResult(Feature feature, string expectedResultFileName)
        {
            string expected = TestFileHelper.ReadFile(expectedResultFileName);
            string got = GenerateCodeFromFeature(feature);

            Assert.AreEqual(expected, got);
        }

        private void GenerateCodeFromFeature(Feature feature, TextWriter writer)
        {
            var converter = new SpecFlowUnitTestConverter(new NUnitTestConverter(),
                                                          new CodeDomHelper(GenerationTargetLanguage.CSharp), true);
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

        [Test]
        public void CanGenerateButFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "but.feature"));
        }

        [Test]
        public void CanGenerateCommentsFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "comments.feature"));
        }

        [Test]
        public void CanGenerateFeatureheaderFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "featureheader.feature"));
        }

        [Test, TestCaseSource(typeof (TestFileHelper), "GetTestFiles")]
        public void CanGenerateFromFile(string fileName)
        {
            Console.WriteLine(fileName);
            var parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StreamReader(fileName))
            {
                Feature feature = parser.Parse(reader);
                Assert.IsNotNull(feature);

                string generatedCode = GenerateCodeFromFeature(feature);
                Assert.IsNotNull(generatedCode);

                // to regenerate the expected result file:
                //GenerateCodeFromFeature(feature, fileName + ".cs");

                //CompareWithExpectedResult(feature, fileName + ".cs");
            }
        }

        [Test]
        public void CanGenerateFromFiles()
        {
            foreach (string testFile in TestFileHelper.GetTestFiles())
            {
                CanGenerateFromFile(testFile);
            }
        }

        [Test]
        public void CanGenerateGivenWhenThenDuplicationFeatureFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "givenwhenthenduplication.feature"));
        }

        [Test]
        public void CanGenerateMixedGWTFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "mixedgivenwhenthen.feature"));
        }

        [Test]
        public void CanGenerateMultilineargumentFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "multilineargument.feature"));
        }

        [Test]
        public void CanGenerateMultilinetitleFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "multilinetitle.feature"));
        }

        [Test]
        public void CanGenerateScneriooutlineFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "scenariooutline.feature"));
        }

        [Test]
        public void CanGenerateSimpleFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "simple.feature"));
        }

        [Test]
        public void CanGenerateTableargumentFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "tableargument.feature"));
        }

        [Test]
        public void CanGenerateTagsFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "tags.feature"));
        }

        [Test]
        public void CanGenerateWhitespacesFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "whitespaces.feature"));
        }

        [Test]
        public void CanGeneratebackgroundFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "background.feature"));
        }

        [Test]
        public void CanGeneratebackgroundWithTitleFeature()
        {
            string folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanGenerateFromFile(Path.Combine(folder, "background_withtitle.feature"));
        }
    }
}