using System.IO;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.Parser;
using TechTalk.SpecFlow.Specs.Support;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class TestFileSteps
    {
        private readonly TestFileManager testFileManager;
        private readonly ParserDriver parserDriver;
        private readonly ProjectsDriver _projectsDriver;

        public TestFileSteps(TestFileManager testFileManager, ParserDriver parserDriver, ProjectsDriver projectsDriver)
        {
            this.testFileManager = testFileManager;
            this.parserDriver = parserDriver;
            _projectsDriver = projectsDriver;
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
            var assemblyFolder = AssemblyFolderHelper.GetAssemblyFolder();

            assemblyFolder.Should().EndWith(@"\bin\Debug\net5.0", "parsed file saving can only be done from a development environment");
            parserDriver.SaveSerializedFeatureTo(Path.Combine(assemblyFolder, @"..\..\..\TestFiles", parsedFileName));
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
                string testFileContent = testFileManager.GetTestFileContent(testFile);

                _projectsDriver.AddFeatureFile(testFileContent);
            }
        }

    }
}
