using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow.IntegrationTests.Drivers;
using TechTalk.SpecFlow.IntegrationTests.TestFiles;
using Should;
using TechTalk.SpecFlow.Assist;
using System.Linq;

namespace TechTalk.SpecFlow.IntegrationTests.StepDefinitions
{
    [Binding]
    public class GherkinParserSteps
    {
        private readonly TestFileManager testFileManager;
        private readonly ParserDriver parserDriver;
        
        public GherkinParserSteps(TestFileManager testFileManager, ParserDriver parserDriver)
        {
            this.testFileManager = testFileManager;
            this.parserDriver = parserDriver;
        }

        [Given(@"there is a Gherkin file as")]
        public void GivenThereIsAGherkinFileAs(string text)
        {
            parserDriver.FileContent = text;
        }

        [When(@"the test file '(.*)' is parsed")]
        public void WhenTheTestFileIsParsed(string testFile)
        {
            string testFileContent = testFileManager.GetTestFileContent(testFile);
            GivenThereIsAGherkinFileAs(testFileContent);
            WhenTheFileIsParsed();
        }

        private string GetAssemblyFolder()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(assemblyFolder != null);
            return assemblyFolder;
        }

        [When(@"the parsed result is saved to '(.*)'")]
        public void WhenTheParsedResultIsSavedTo(string parsedFileName)
        {
            var assemblyFolder = GetAssemblyFolder();
            assemblyFolder.EndsWith(@"\bin\Debug").ShouldBeTrue("parsed file saving can only be done from a development environment");
            parserDriver.SaveSerializedFeatureTo(Path.Combine(assemblyFolder, @"..\..\TestFiles", parsedFileName));
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

        [Then(@"the the following errors are provided")]
        public void ThenTheTheFollowingErrorsAreProvided(List<ExpectedError> expectedErrors)
        {
            parserDriver.AssertErrors(expectedErrors);
        }

        [Then(@"the parsed result is the same as '(.*)'")]
        public void ThenTheParsedResultIsTheSameAs(string parsedFileName)
        {
            string expected = testFileManager.GetTestFileContent(parsedFileName);
            parserDriver.AssertParsedFeatureEqualTo(expected);
        }

    }
}
