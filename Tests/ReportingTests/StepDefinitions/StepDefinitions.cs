using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using NUnit.Framework;
using SpecFlowTool = TechTalk.SpecFlow.Tools.Program;

namespace ReportingTests.StepDefinitions
{
    [Binding]
    public class StepDefinitions
    {
        private SampleProjectInfo sampleProjectInfo;

        public StepDefinitions(SampleProjectInfo sampleProjectInfo)
        {
            this.sampleProjectInfo = sampleProjectInfo;
        }

        [Given(@"there are NUuit test execution results for the ReportingTest.SampleProject project")]
        public void GivenThereIsAnNUuitTestExecutionResultsForMyProject()
        {
            Assert.IsTrue(File.Exists(sampleProjectInfo.NUnitXmlResultPath), "NUnit xml test result is missing");
            Assert.IsTrue(File.Exists(sampleProjectInfo.NUnitTextResultPath), "NUnit txt test result is missing");
        }

        [Given(@"there is an XSLT template containing")]
        public void GivenThereIsAnXSLTTemplateContaining(string xsltContent)
        {
            sampleProjectInfo.CustomXslt = GenerateCustomXslt(xsltContent, null);
        }

        [Given(@"there is an XSLT template '(.*)' containing")]
        public void GivenThereIsAnXSLTTemplateContaining(string templateFileName, string xsltContent)
        {
            GenerateCustomXslt(xsltContent, templateFileName);
        }

        [When(@"I generate SpecFlow NUnit execution report with the custom XSLT")]
        public void WhenIGenerateSpecFlowNUnitExecutionReportWithTheCustomXSLT()
        {
            SpecFlowTool.NUnitExecutionReport(
                sampleProjectInfo.ProjectFilePath, 
                sampleProjectInfo.NUnitXmlResultPath, 
                sampleProjectInfo.CustomXslt, 
                sampleProjectInfo.NUnitTextResultPath,
                sampleProjectInfo.OutputFilePath);
        }

        [Then(@"a report generated like")]
        public void ThenAReportGeneratedLike(string expectedResultContent)
        {
            Assert.IsTrue(File.Exists(sampleProjectInfo.OutputFilePath), "no result is generated");

            string resultContent = sampleProjectInfo.GetOutputFileContent();

            AssertEqualIgnoringWhitespace(expectedResultContent, resultContent);
        }

        private void AssertEqualIgnoringWhitespace(string expectedValue, string actualValue)
        {
            StringAssert.AreEqualIgnoringCase(NormalizeWhitespace(expectedValue), NormalizeWhitespace(actualValue));
        }

        private string NormalizeWhitespace(string value)
        {
            var whitespaceRe = new Regex(@"\s+");
            return whitespaceRe.Replace(value.Trim(), " ");
        }

        private string GenerateCustomXslt(string content, string templateFileName)
        {
            string xsltTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
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
                              ? sampleProjectInfo.GenerateTempFilePath(".xslt")
                              : sampleProjectInfo.GenerateFilePath(templateFileName);
            using (TextWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine(fullContent);
            }

            return path;
        }
    }
}