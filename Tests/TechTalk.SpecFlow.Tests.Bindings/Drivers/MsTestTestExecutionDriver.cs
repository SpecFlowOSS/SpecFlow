﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers
{
    public class MsTestTestExecutionDriver
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly TestExecutionResult testExecutionResult;

        public TestSettingsFileInput TestSettingsFile { get; set; }

        public MsTestTestExecutionDriver(InputProjectDriver inputProjectDriver, TestExecutionResult testExecutionResult)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.testExecutionResult = testExecutionResult;
        }

        public TestRunSummary Execute()
        {
            string vsFolder = Environment.Is64BitProcess ? @"%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE" : @"%ProgramFiles%\Microsoft Visual Studio 14.0\Common7\IDE";
            var msTestConsolePath = Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(),
                Environment.ExpandEnvironmentVariables(vsFolder + @"\MsTest.exe"));

            string resultsFilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "mstest-result.trx");

            string testSettingsFilePath = null;

            var processHelper = new ProcessHelper();

            string argumentsFormat = "\"/testcontainer:{0}\" \"/resultsfile:{1}\" /usestderr";

            if (this.TestSettingsFile != null)
            {
                testSettingsFilePath = Path.Combine(
                    inputProjectDriver.CompilationFolder,
                    this.TestSettingsFile.ProjectRelativePath);

                File.WriteAllText(testSettingsFilePath, this.TestSettingsFile.Content, Encoding.UTF8);

                argumentsFormat += " \"/testsettings:{2}\"";
            }

            processHelper.RunProcess(msTestConsolePath, argumentsFormat, 
                inputProjectDriver.CompiledAssemblyPath, resultsFilePath, testSettingsFilePath);

            XDocument logFile = XDocument.Load(resultsFilePath);

            TestRunSummary summary = new TestRunSummary();

            XmlNameTable nameTable = new NameTable();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("mstest", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            var summaryElement = logFile.XPathSelectElement("//mstest:ResultSummary/mstest:Counters", namespaceManager);
            if (summaryElement != null)
            {
                summary.Total = int.Parse(summaryElement.Attribute("total").Value);
                summary.Succeeded = int.Parse(summaryElement.Attribute("passed").Value);
                summary.Failed = int.Parse(summaryElement.Attribute("failed").Value);
                summary.Pending = int.Parse(summaryElement.Attribute("inconclusive").Value);
                summary.Ignored = 0; // mstest does not support ignored in the report
            }

            testExecutionResult.LastExecutionSummary = summary;

            return summary;
        }
    }
}
