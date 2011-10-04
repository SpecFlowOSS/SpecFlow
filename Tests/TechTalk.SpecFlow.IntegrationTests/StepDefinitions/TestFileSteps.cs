using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow.IntegrationTests.TestFiles;
using Should;
using TechTalk.SpecFlow.Assist;
using System.Linq;
using TechTalk.SpecFlow.Specs.Drivers.Parser;

namespace TechTalk.SpecFlow.IntegrationTests.StepDefinitions
{
    [Binding]
    public class TestFileSteps
    {
        private readonly TestFileManager testFileManager;
        private readonly ParserDriver parserDriver;
        
        public TestFileSteps(TestFileManager testFileManager, ParserDriver parserDriver)
        {
            this.testFileManager = testFileManager;
            this.parserDriver = parserDriver;
        }

        [When(@"the test file '(.*)' is parsed")]
        public void WhenTheTestFileIsParsed(string testFile)
        {
            string testFileContent = testFileManager.GetTestFileContent(testFile);
            parserDriver.FileContent = testFileContent;
            parserDriver.ParseFile();
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

        [Then(@"the parsed result is the same as '(.*)'")]
        public void ThenTheParsedResultIsTheSameAs(string parsedFileName)
        {
            string expected = testFileManager.GetTestFileContent(parsedFileName);
            parserDriver.AssertParsedFeatureEqualTo(expected);
        }
    }
}
