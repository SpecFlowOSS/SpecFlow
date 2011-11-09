using System.Collections.Generic;
using Should;
using TechTalk.SpecFlow.Assist;
using System.Linq;
using TechTalk.SpecFlow.Specs.Drivers.Parser;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class GherkinParserSteps
    {
        private readonly ParserDriver parserDriver;
        
        public GherkinParserSteps(ParserDriver parserDriver)
        {
            this.parserDriver = parserDriver;
        }

        [Given(@"there is a Gherkin file as")]
        public void GivenThereIsAGherkinFileAs(string text)
        {
            parserDriver.FileContent = text;
        }

        [When(@"the file is parsed")]
        public void WhenTheFileIsParsed()
        {
            parserDriver.ParseFile();
        }

        [Then(@"no parsing error is reported")]
        public void ThenNoParsingErrorIsReported()
        {
            parserDriver.ParsingErrors.ShouldEqual(null, "There are parsing errors");
        }

        [StepArgumentTransformation]
        public List<ExpectedError> ConvertExpectedErrors(Table table)
        {
            return table.CreateSet<ExpectedError>().ToList();
        }

        [Then(@"the following errors are provided")]
        public void ThenTheTheFollowingErrorsAreProvided(List<ExpectedError> expectedErrors)
        {
            parserDriver.AssertErrors(expectedErrors);
        }
    }
}
