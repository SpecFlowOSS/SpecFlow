using System.IO;
using TechTalk.SpecFlow.IntegrationTests.TestFiles;
using FluentAssertions;
using SpecFlow.TestProjectGenerator.NewApi;
using SpecFlow.TestProjectGenerator.NewApi.Driver;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.Parser;

namespace TechTalk.SpecFlow.IntegrationTests.StepDefinitions
{
    [Binding]
    public class TestFileSteps
    {
        private readonly TestFileManager _testFileManager;
        private readonly ProjectsDriver _projectsDriver;
        private readonly ParserDriver _parserDriver;
        private readonly TestProjectFolders _testProjectFolders;

        public TestFileSteps(TestFileManager testFileManager, ProjectsDriver projectsDriver, ParserDriver parserDriver, TestProjectFolders testProjectFolders)
        {
            _testFileManager = testFileManager;
            _projectsDriver = projectsDriver;
            _parserDriver = parserDriver;
            _testProjectFolders = testProjectFolders;
        }

        [When(@"the test file '(.*)' is parsed")]
        public void WhenTheTestFileIsParsed(string testFile)
        {
            string testFileContent = _testFileManager.GetTestFileContent(testFile);
            _parserDriver.FileContent = testFileContent;
            _parserDriver.ParseFile();
        }

        [When(@"the parsed result is saved to '(.*)'")]
        public void WhenTheParsedResultIsSavedTo(string parsedFileName)
        {
            _parserDriver.SaveSerializedFeatureTo(Path.Combine(_testProjectFolders.ProjectFolder, @"TestFiles", parsedFileName));
        }

        [Then(@"the parsed result is the same as '(.*)'")]
        public void ThenTheParsedResultIsTheSameAs(string parsedFileName)
        {
            string expected = _testFileManager.GetTestFileContent(parsedFileName);
            _parserDriver.AssertParsedFeatureEqualTo(expected);
        }

        [Given(@"all test files are inluded in the project")]
        public void GivenAllTestFilesAreInludedInTheProject()
        {
            foreach (string testFile in _testFileManager.GetTestFeatureFiles())
            {
                _projectsDriver.AddFile(testFile, _testFileManager.GetTestFileContent(testFile));
            }
        }
    }
}
