using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TechTalk.SpecFlow.Specs.Drivers;
using Should;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ReportingSteps 
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly ProjectSteps projectSteps;
        private readonly ExecutionSteps executionSteps;
        private readonly SpecFlowConfigurationDriver specFlowConfigurationDriver;
        private readonly NUnitTestExecutionDriver nUnitTestExecutionDriver;
        private readonly ReportInfo reportInfo;

        public ReportingSteps(InputProjectDriver inputProjectDriver, ProjectSteps projectSteps, SpecFlowConfigurationDriver specFlowConfigurationDriver, ExecutionSteps executionSteps, ReportInfo reportInfo)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.reportInfo = reportInfo;
            this.executionSteps = executionSteps;
            this.projectSteps = projectSteps;
            this.specFlowConfigurationDriver = specFlowConfigurationDriver;
        }

        [Given(@"there are (.*) test execution results for the project")]
        public void GivenThereAreNUnitTestExecutionResultsForTheProject(string unitTestProvider)
        {
            specFlowConfigurationDriver.SetUnitTestProvider(unitTestProvider);
            projectSteps.EnsureCompiled();

            executionSteps.WhenIExecuteTheTestsWith(unitTestProvider);
        }

        [When(@"I generate SpecFlow NUnit execution report")]
        [When(@"I generate SpecFlow NUnit execution report with the custom XSLT")]
        public void WhenIGenerateSpecFlowNUnitExecutionReport()
        {
            ProcessHelper processHelper = new ProcessHelper();

            reportInfo.FilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "executionreport.html");

            processHelper.RunProcess(
                Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), @"SpecFlow\tools\specflow.exe"),
                "nunitexecutionreport \"{0}\" \"/xmlTestResult:{1}\" \"/testOutput:{2}\" \"/out:{3}\" {4}", inputProjectDriver.ProjectFilePath,
                Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.xml"), Path.Combine(inputProjectDriver.DeploymentFolder, "nunit-result.txt"),
                reportInfo.FilePath, GetCustomXsltArgument());
        }

        [When(@"I generate SpecFlow MsTest execution report")]
        [When(@"I generate SpecFlow MsTest execution report with the custom XSLT")]
        public void WhenIGenerateSpecFlowMsTestExecutionReport()
        {
            ProcessHelper processHelper = new ProcessHelper();

            reportInfo.FilePath = Path.Combine(inputProjectDriver.DeploymentFolder, "executionreport.html");

            processHelper.RunProcess(
                Path.Combine(AssemblyFolderHelper.GetTestAssemblyFolder(), @"SpecFlow\tools\specflow.exe"),
                "mstestexecutionreport \"{0}\" \"/testResult:{1}\" \"/out:{2}\" {3}", inputProjectDriver.ProjectFilePath,
                Path.Combine(inputProjectDriver.DeploymentFolder, "mstest-result.trx"), reportInfo.FilePath, GetCustomXsltArgument());
        }

        private string customXslt;

        private string GetCustomXsltArgument()
        {
            return customXslt == null ? "" : string.Format(" \"/xsltFile:{0}\"", customXslt);
        }

        [Given(@"there is an XSLT template containing")]
        public void GivenThereIsAnXSLTTemplateContaining(string xsltContent)
        {
            customXslt = GenerateCustomXslt(xsltContent, null);
        }

        [Given(@"there is an XSLT template '(.*)' containing")]
        public void GivenThereIsAnXSLTTemplateContaining(string templateFileName, string xsltContent)
        {
            GenerateCustomXslt(xsltContent, templateFileName);
        }

        private string GenerateCustomXslt(string content, string templateFileName)
        {
            const string xsltTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xsl:stylesheet version=""1.0"" 
                xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""
                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" 
                xmlns:sfr=""urn:TechTalk:SpecFlow.Report""
                xmlns:nunit=""urn:NUnit""
                exclude-result-prefixes=""msxsl"">
  <xsl:output method=""html"" doctype-system=""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"" doctype-public=""-//W3C//DTD XHTML 1.0 Transitional//EN""/>

{0}

</xsl:stylesheet>
";

            string fullContent = string.Format(xsltTemplate, content);

            string path = templateFileName == null
                              ? GenerateTempFilePath(".xslt")
                              : GenerateFilePath(templateFileName);
            using (TextWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine(fullContent);
            }

            return path;
        }

        public string GenerateFilePath(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        public string GenerateTempFilePath(string extension)
        {
            return Path.Combine(Path.GetTempPath(), Path.GetTempFileName()) + extension;
        }

        [Then(@"the generated report contains")]
        public void ThenTheGeneratedReportContains(string expectedResultContent)
        {
            reportInfo.AssertExists();
            reportInfo.AssertContainsIgnoringWhitespace(expectedResultContent);
        }

        [Then(@"a report generated like")]
        public void ThenAReportGeneratedLike(string expectedResultContent)
        {
            reportInfo.AssertExists();
            reportInfo.AssertEqualIgnoringWhitespace(expectedResultContent);
        }
    }
}
