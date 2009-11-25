/*using System;
using System.Collections.Generic;
using System.IO;
using NConsoler;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport;
using TechTalk.SpecFlow.Reporting.StepDefinitionReport;

namespace TechTalk.SpecFlow.Reporting
{
    class Program
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
            return;
        }

        [Action("Step Definition Report")]
        public static void StepDefinitionReport(
            [Required] string projectFile, 
            [Optional("bin\\Debug")] string binFolder,
            [Optional("StepDefinitionReport.html", "out")] string outputFile
            )
        {
            StepDefinitionReportGenerator generator = new StepDefinitionReportGenerator(projectFile, binFolder, true);
            generator.GenerateReport();
            generator.TransformReport(Path.GetFullPath(outputFile));
        }

        [Action]
        public static void NUnitExecutionReport(
            [Required] string projectFile,
            [Optional("TestResult.xml")] string xmlTestResult,
            [Optional("TestResult.txt", "testOutput")] string labeledTestOutput,
            [Optional("TestResult.html", "out")] string outputFile
            )
        {
            NUnitExecutionReportGenerator generator = new NUnitExecutionReportGenerator(
                projectFile, 
                Path.GetFullPath(xmlTestResult),
                Path.GetFullPath(labeledTestOutput));
            generator.GenerateReport();
            generator.TransformReport(Path.GetFullPath(outputFile));
        }
    }
}*/