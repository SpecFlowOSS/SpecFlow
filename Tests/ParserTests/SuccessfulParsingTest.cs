using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace ParserTests
{
    [TestFixture]
    public class SuccessfulParsingTest
    {
        [Test]
        public void CanParseFiles()
        {
            foreach (var testFile in TestFileHelper.GetTestFiles())
            {
                CanParseFile(testFile);
            }
        }

        [Test]
        public void CanParseSimpleFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "simple.feature"));
        }       
        
        [Test]
        public void CanGenerateAsterisksFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "asterisks.feature"));
        }

        [Test]
        public void CanParseGermanFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "german.feature"));
        }

        [Test]
        public void CanParseHungarianFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "hungarian.feature"));
        }

        [Test]
        public void CanParseSwedishFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "swedish.feature"));
        }

        [Test]
        public void CanParseCommentsFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "comments.feature"));
        }

        [Test]
        public void CanParseFeatureheaderFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "featureheader.feature"));
        }
 
        [Test]
        public void CanParseTagsFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "tags.feature"));
        }
 
        [Test]
        public void CanParseTaggedExamplesFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "taggedexamples.feature"));
        }

        [Test]
        public void CanParsebackgroundFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "background.feature"));
        }

        [Test]
        public void CanParsebackgroundWithTitleFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "background_withtitle.feature"));
        }

        [Test]
        public void CanParseWhitespacesFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "whitespaces.feature"));
        }

        [Test]
        public void CanParseGivenWhenThenDuplicationFeatureFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "givenwhenthenduplication.feature"));
        }

        [Test]
        public void CanParseButFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "but.feature"));
        }

        [Test]
        public void CanParseMultilinetitleFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "multilinetitle.feature"));
        }

        [Test]
        public void CanParseMultilineargumentFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "multilineargument.feature"));
        }

        [Test]
        public void CanParseTableargumentFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "tableargument.feature"));
        }

        [Test]
        public void CanParseScenarioOutlineFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "scenariooutline.feature"));
        }

        [Test]
        public void CanParseMixedGWTFeature()
        {
            var folder = Path.GetFullPath(Path.Combine(TestFileHelper.GetProjectLocation(), "TestFiles"));
            CanParseFile(Path.Combine(folder, "mixedgivenwhenthen.feature"));
        }

        [Test, TestCaseSource(typeof(TestFileHelper), "GetTestFiles")]
        public void CanParseFile(string fileName)
        {
            Console.WriteLine(fileName);
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StreamReader(fileName))
            {
                Feature feature = parser.Parse(reader, fileName);
                Assert.IsNotNull(feature);
                Assert.AreEqual(fileName, feature.SourceFile);

                feature.SourceFile = null; // cleanup source file to make the test run from other folders too

                // to regenerate the expected result file:
                //SerializeFeature(feature, fileName + ".xml");

                CompareWithExpectedResult(feature, fileName + ".xml");
            }
        }

        private void CompareWithExpectedResult(Feature feature, string expectedResultFileName)
        {
            string expected = TestFileHelper.ReadFile(expectedResultFileName).Replace("\r", "");
            string got = SerializeFeature(feature).Replace("\r", "");

            Assert.AreEqual(expected, got);
        }

        private void SerializeFeature(Feature feature, TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Feature));
            serializer.Serialize(writer, feature);
        }

        private void SerializeFeature(Feature feature, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                SerializeFeature(feature, writer);
            }
        }

        private string SerializeFeature(Feature feature)
        {
            using (var writer = new Utf8StringWriter())
            {
                SerializeFeature(feature, writer);
                return writer.ToString();
            }
        }
    }
}
