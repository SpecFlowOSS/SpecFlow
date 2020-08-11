using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Gherkin;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Specs.Drivers.Parser
{
    public class ParserDriver
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly SpecFlowGherkinParser _parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
        public string FileContent { get; set; }
        public SpecFlowDocument ParsedDocument { get; private set; }
        public ParserException[] ParsingErrors { get; private set; }

        public void ParseFile()
        {
            var contentReader = new StringReader(FileContent);
            ParsedDocument = null;
            ParsingErrors = new ParserException[0];

            try
            {
                ParsedDocument = _parser.Parse(contentReader, new SpecFlowDocumentLocation("sample.feature"));
                ParsedDocument.Should().NotBeNull();                
            }
            catch (ParserException ex)
            {
                ParsingErrors = ex.GetParserExceptions();
                Console.WriteLine("-> parsing errors");
                foreach (var error in ParsingErrors)
                {
                    Console.WriteLine("-> {0}:{1} {2}", error.Location?.Line ?? 0, error.Location?.Column ?? 0, error.Message);
                }
            }
        }

        public void AssertParsedFeatureEqualTo(string parsedFeatureXml)
        {
            const string ns1 = "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
            const string ns2 = "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";

            string expected = parsedFeatureXml.Replace("\r", "").Replace(ns1, "").Replace(ns2, "");
            string got = SerializeDocument(ParsedDocument).Replace("\r", "").Replace(ns1, "").Replace(ns2, "");

            got.Should().Be(expected);
        }

        public void AssertErrors(List<ExpectedError> expectedErrors)
        {
            expectedErrors.Should().NotBeEmpty("please specify expected errors");
            
            ParsingErrors.Should().NotBeEmpty("The parsing was successful");
            

            foreach (var expectedError in expectedErrors)
            {
                string message = expectedError.Error.ToLower();

                var errorDetail =
                    ParsingErrors.FirstOrDefault(ed => ed.Location != null && ed.Location.Line == expectedError.Line &&
                        ed.Message.ToLower().Contains(message));

                errorDetail.Should().NotBeNull("no such error: {0}", message);
            }
        }

        public void SaveSerializedFeatureTo(string fileName)
        {
            ParsedDocument.Should().NotBeNull("The parsing was not successful");
            SerializeDocument(ParsedDocument, fileName);
        }

        private void SerializeDocument(SpecFlowDocument feature, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                SerializeDocument(feature, writer);
            }
        }

        private string SerializeDocument(SpecFlowDocument feature)
        {
            using (var writer = new Utf8StringWriter())
            {
                SerializeDocument(feature, writer);
                return writer.ToString();
            }
        }

        private void SerializeDocument(SpecFlowDocument feature, TextWriter writer)
        {
            _serializer.Serialize(writer, feature);
        }
    }

    public class ExpectedError
    {
        public int? Line { get; set; }
        public string Error { get; set; }
    }
}
