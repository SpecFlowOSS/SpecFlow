using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Specs.Drivers.Parser
{
    public class ParserDriver
    {
        public string FileContent { get; set; }
        public Feature ParsedFeature { get; private set; }
        public SpecFlowParserException ParsingErrors { get; private set; }

        private readonly SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));

        public void ParseFile()
        {
            var contentReader = new StringReader(FileContent);
            ParsedFeature = null;
            ParsingErrors = null;

            try
            {
                ParsedFeature = parser.Parse(contentReader, "sample.feature");
                Assert.IsNotNull(ParsedFeature);
                ParsedFeature.SourceFile = null;
            }
            catch (SpecFlowParserException ex)
            {
                ParsingErrors = ex;
                Console.WriteLine("-> parsing errors");
                foreach (ErrorDetail errorDetail in ParsingErrors.ErrorDetails)
                {
                    Console.WriteLine("-> {0}:{1} {2}", errorDetail.Line, errorDetail.Column, errorDetail.Message);
                }
            }
        }

        public void AssertParsedFeatureEqualTo(string parsedFeatureXml)
        {
            const string NS1 = "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
            const string NS2 = "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";

            string expected = parsedFeatureXml.Replace("\r", "").Replace(NS1, "").Replace(NS2, "");
            string got = SerializeFeature(ParsedFeature).Replace("\r", "").Replace(NS1, "").Replace(NS2, "");

            Assert.AreEqual(expected, got);
        }

        public void AssertErrors(List<ExpectedError> expectedErrors)
        {
            Assert.Greater(expectedErrors.Count, 0, "please specify expected errors");

            Assert.IsNotNull(ParsingErrors, "The parsing was successful");

            foreach (var expectedError in expectedErrors)
            {
                string message = expectedError.Error.ToLower();

                var errorDetail =
                    ParsingErrors.ErrorDetails.Find(ed => ed.Line == expectedError.Line &&
                        ed.Message.ToLower().Contains(message));

                Assert.IsNotNull(errorDetail, "no such error: {0}", message);
            }
        }

        private void SerializeFeature(Feature feature, TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Feature));
            serializer.Serialize(writer, feature);
        }

        public void SaveSerializedFeatureTo(string fileName)
        {
            Assert.IsNotNull(ParsedFeature, "The parsing was not successful");
            SerializeFeature(ParsedFeature, fileName);
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

    public class ExpectedError
    {
        public int? Line { get; set; }
        public string Error { get; set; }
    }
}
