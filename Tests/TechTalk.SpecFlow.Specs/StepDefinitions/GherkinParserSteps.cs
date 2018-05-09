using System.Collections.Generic;
using TechTalk.SpecFlow.Assist;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers.Parser;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class GherkinParserSteps
    {
        private readonly ParserDriver _parserDriver;
        
        public GherkinParserSteps(ParserDriver parserDriver)
        {
            _parserDriver = parserDriver;
        }

        [Given(@"there is a Gherkin file as")]
        public void GivenThereIsAGherkinFileAs(string text)
        {
            _parserDriver.FileContent = text;
        }

        [When(@"the file is parsed")]
        public void WhenTheFileIsParsed()
        {
            _parserDriver.ParseFile();
        }

        [Then(@"no parsing error is reported")]
        public void ThenNoParsingErrorIsReported()
        {
            _parserDriver.ParsingErrors.Should().BeEmpty("There are parsing errors");
        }

        [StepArgumentTransformation]
        public List<ExpectedError> ConvertExpectedErrors(Table table)
        {
            return table.CreateSet<ExpectedError>().ToList();
        }

        [Then(@"the following errors are provided")]
        public void ThenTheTheFollowingErrorsAreProvided(List<ExpectedError> expectedErrors)
        {
            _parserDriver.AssertErrors(expectedErrors);
        }
    }
}
