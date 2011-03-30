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

        [Given(@"there are NUnit test execution results for the ReportingTest.SampleProject project")]
        public void GivenThereIsAnNUuitTestExecutionResultsForMyProject()
        {
            Assert.IsTrue(File.Exists(sampleProjectInfo.NUnitXmlResultPath), "NUnit xml test result is missing! Expected file: " + sampleProjectInfo.NUnitXmlResultPath);
            Assert.IsTrue(File.Exists(sampleProjectInfo.NUnitTextResultPath), "NUnit txt test result is missing! Expected file: " + sampleProjectInfo.NUnitTextResultPath);
        }

        [Given(@"there are MsTest test execution results for the ReportingTest.SampleProject project")]
        public void GivenThereIsAnMsTestTestExecutionResultsForMyProject()
        {
            Assert.IsTrue(File.Exists(sampleProjectInfo.MsTestResultPath), "MsTest test result is missing! Exxpected file: " + sampleProjectInfo.MsTestResultPath);
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

        [When(@"I generate SpecFlow NUnit execution report( with the custom XSLT)?")]
        public void WhenIGenerateSpecFlowNUnitExecutionReportWithTheCustomXSLT(string withCustomXslt)
        {
            SpecFlowTool.NUnitExecutionReport(
                sampleProjectInfo.ProjectFilePath, 
                sampleProjectInfo.NUnitXmlResultPath, 
                sampleProjectInfo.CustomXslt, 
                sampleProjectInfo.NUnitTextResultPath,
                sampleProjectInfo.OutputFilePath);
        }

        [When(@"I generate SpecFlow MsTest execution report( with the custom XSLT)?")]
        public void WhenIGenerateSpecFlowMsTestExecutionReportWithTheCustomXSLT(string withCustomXslt)
        {
            SpecFlowTool.MsTestExecutionReport(
                sampleProjectInfo.ProjectFilePath, 
                sampleProjectInfo.MsTestResultPath, 
                sampleProjectInfo.CustomXslt, 
                sampleProjectInfo.OutputFilePath);
        }

        [Then(@"a report generated like")]
        public void ThenAReportGeneratedLike(string expectedResultContent)
        {
            Assert.IsTrue(File.Exists(sampleProjectInfo.OutputFilePath), "no result is generated");

            string resultContent = sampleProjectInfo.GetOutputFileContent();

            AssertEqualIgnoringWhitespace(expectedResultContent, resultContent);
        }

        [Then(@"a report generated containing")]
        public void ThenAReportGeneratedContaining(string expectedResultContent)
        {
            Assert.IsTrue(File.Exists(sampleProjectInfo.OutputFilePath), "no result is generated");

            string resultContent = sampleProjectInfo.GetOutputFileContent();

            AssertContainsIgnoringWhitespace(expectedResultContent, resultContent);
        }

        private void AssertEqualIgnoringWhitespace(string expectedValue, string actualValue)
        {
            StringAssert.AreEqualIgnoringCase(
                NormalizeWhitespace(CleanHtml(expectedValue)), 
                NormalizeWhitespace(CleanHtml(actualValue)));
        }

        private void AssertContainsIgnoringWhitespace(string expectedValue, string actualValue)
        {
            StringAssert.Contains(
                NormalizeWhitespace(HtmlEncode(expectedValue)).ToLowerInvariant(),
                NormalizeWhitespace(CleanHtml(actualValue)).ToLowerInvariant());
        }

        private string NormalizeWhitespace(string value)
        {
            var whitespaceRe = new Regex(@"\s+");
            return whitespaceRe.Replace(value.Trim(), " ");
        }

        private string HtmlEncode(string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string CleanHtml(string value)
        {
            var bodyRe = new Regex(@"\<\/?\s*body\s*\>");
            var bodyMatch = bodyRe.Match(value);
            if (bodyMatch.Success)
            {
                value = value.Substring(bodyMatch.Index + bodyMatch.Value.Length);
                bodyMatch = bodyRe.Match(value);
                if (bodyMatch.Success)
                    value = value.Substring(0, bodyMatch.Index);
            }
            var htmlTagRe = new Regex(@"\<.*?\>");
            value = htmlTagRe.Replace(value.Trim(), " ");

            var nbspRe = new Regex(@"\&nbsp;", RegexOptions.IgnoreCase);
            value = nbspRe.Replace(value, " ");

            return value;
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