using System.IO;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.Parser;
using TechTalk.SpecFlow.Specs.Support;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class TestFileSteps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly TestFileManager testFileManager;
        private readonly ParserDriver parserDriver;
        
        public TestFileSteps(TestFileManager testFileManager, ParserDriver parserDriver, InputProjectDriver inputProjectDriver)
        {
            this.testFileManager = testFileManager;
            this.inputProjectDriver = inputProjectDriver;
            this.parserDriver = parserDriver;
        }

        [When(@"the test file '(.*)' is parsed")]
        public void WhenTheTestFileIsParsed(string testFile)
        {
            string testFileContent = testFileManager.GetTestFileContent(testFile);
            parserDriver.FileContent = testFileContent;
            parserDriver.ParseFile();
        }

        [When(@"the parsed result is saved to '(.*)'")]
        public void WhenTheParsedResultIsSavedTo(string parsedFileName)
        {
            var assemblyFolder = AssemblyFolderHelper.GetTestAssemblyFolder();
            assemblyFolder.EndsWith(@"\bin\Debug").Should().BeTrue("parsed file saving can only be done from a development environment");
            parserDriver.SaveSerializedFeatureTo(Path.Combine(assemblyFolder, @"..\..\TestFiles", parsedFileName));
        }

        [Then(@"the parsed result is the same as '(.*)'")]
        public void ThenTheParsedResultIsTheSameAs(string parsedFileName)
        {
            string expected = testFileManager.GetTestFileContent(parsedFileName);
            parserDriver.AssertParsedFeatureEqualTo(expected);
        }

        [Given(@"all test files are inluded in the project")]
        public void GivenAllTestFilesAreInludedInTheProject()
        {
            foreach (var testFile in testFileManager.GetTestFeatureFiles())
            {
                inputProjectDriver.AddFeatureFile(testFileManager.GetTestFileContent(testFile), testFile);
            }
        }

    }
}
