using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace ReportingTests.StepDefinitions
{
    [Binding]
    public class Setup
    {
        [BeforeScenario]
        public void BeforeScenario()
        {
            //ScenarioContext.Current["project"]
        }
    }

    public class SampleProjectInfo : IDisposable
    {
        public string ProjectFolder { get; private set; }
        public string NUnitXmlResultPath { get; private set; }
        public string NUnitTextResultPath { get; private set; }
        public string MsTestResultPath { get; private set; }
        public string CustomXslt { get; set; }

        public string OutputFilePath { get; private set; }

        public string ProjectFilePath { get; private set; }


        public SampleProjectInfo()
        {
			string projectFolderLocation = String.Format("..{0}..{0}..{0}ReportingTest.SampleProject", Path.DirectorySeparatorChar);
            ProjectFolder = Path.GetFullPath(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), projectFolderLocation));
            ProjectFilePath = Path.Combine(ProjectFolder, @"ReportingTest.SampleProject.csproj");

            NUnitXmlResultPath = Path.Combine(ProjectFolder, String.Format("NUnitResult{0}TestResult.xml", Path.DirectorySeparatorChar));
            NUnitTextResultPath = Path.Combine(ProjectFolder, String.Format("NUnitResult{0}TestResult.txt", Path.DirectorySeparatorChar));
            MsTestResultPath = Path.Combine(ProjectFolder, String.Format("MsTestResult{0}TestResult.trx", Path.DirectorySeparatorChar));

            OutputFilePath = GenerateTempFilePath(".html");
        }

        public string GenerateFilePath(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        public string GenerateTempFilePath(string extension)
        {
            return Path.Combine(Path.GetTempPath(), Path.GetTempFileName()) + extension;
        }

        public void Dispose()
        {
            if (CustomXslt != null)
                File.Delete(CustomXslt);
        }

        public string GetOutputFileContent()
        {
            Assert.IsTrue(File.Exists(OutputFilePath), "output file is missing");                

            using (var reader = new StreamReader(OutputFilePath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
