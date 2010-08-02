using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using Table = TechTalk.SpecFlow.Table;

namespace ParserTests.StepDefinitions
{
    [Binding]
    public class GherkinParserSteps
    {
        private SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
        private string gherkinContent;
        private Feature feature;
        private SpecFlowParserException parsingErrors;

        [Given(@"there is a Gherkin file as")]
        public void GivenThereIsAGherkinFileAs(string text)
        {
            gherkinContent = text;
        }

        [When(@"I parse the file")]
        public void WhenIParseTheFile()
        {
            var contentReader = new StringReader(gherkinContent);

            try
            {
                feature = parser.Parse(contentReader, "sample.feature");
            }
            catch(SpecFlowParserException ex)
            {
                parsingErrors = ex;
                Console.WriteLine("-> parsing errors");
                foreach (ErrorDetail errorDetail in parsingErrors.ErrorDetails)
                {
                    Console.WriteLine("-> {0}:{1} {2}", errorDetail.Line, errorDetail.Column, errorDetail.Message);
                }
            }
        }

        [Then(@"the the following errors are provided")]
        public void ThenTheTheFollowingErrorsAreProvided(Table table)
        {
            Assert.Greater(table.Rows.Count, 0, "please specify expected errors");
            
            Assert.IsNotNull(parsingErrors, "The parsing was successful");

            foreach (TableRow expectedError in table.Rows)
            {
                int? line = expectedError["line"].Trim().Length == 0 ? null :
                    (int?)int.Parse(expectedError["line"].Trim());
                string message = expectedError["error"].ToLower();

                var errorDetail =
                    parsingErrors.ErrorDetails.Find(ed => ed.Line == line &&
                        ed.Message.ToLower().Contains(message));

                Assert.IsNotNull(errorDetail, "no such error: {0}", message);
            }
        }
    }
}
