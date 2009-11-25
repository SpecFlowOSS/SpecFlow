using System;
using System.IO;
using NConsoler;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport;
using TechTalk.SpecFlow.Reporting.StepDefinitionReport;

namespace TechTalk.SpecFlow.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
            return;
        }

        [Action("Generate tests from all feature files in a project")]
        public static void GenerateAll(
            [Required] string projectFile,
            [Optional(false, "force", "f")] bool forceGeneration,
            [Optional(false, "verbose", "v")] bool verboseOutput
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            BatchGenerator batchGenerator = new BatchGenerator(Console.Out, verboseOutput);
            batchGenerator.ProcessProject(specFlowProject, forceGeneration);
        }

        #region Reports

        [Action("Generates a report about usage and binding of steps")]
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

        [Action("Formats an NUnit execution report to SpecFlow style")]
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

        #endregion
    }
}