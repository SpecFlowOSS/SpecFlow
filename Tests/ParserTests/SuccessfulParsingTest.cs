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
