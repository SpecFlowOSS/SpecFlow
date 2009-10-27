using System;
using System.Collections.Generic;
using System.IO;
using NConsoler;
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
            [Optional("StepDefinitionReport.html", "out")] string outputFile)
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            List<Feature> parsedFeatures = ParserHelper.GetParsedFeatures(specFlowProject);

            var basePath = Path.Combine(specFlowProject.ProjectFolder, binFolder);
            List<BindingInfo> bindings = BindingCollector.CollectBindings(specFlowProject, basePath);

            StepDefinitionReportGenerator generator = new StepDefinitionReportGenerator(specFlowProject, bindings, parsedFeatures);
            generator.GenerateReport();

            string outputFilePath = Path.Combine(basePath, outputFile);
            generator.TransformReport(outputFilePath);
        }

        [Action]
        public static void NUnitExecutionReport(
            [Optional("TestResult.xml")] string xmlTestResult,
            [Optional("TestResult.txt", "testOutput")] string labeledTestOutput,
            [Optional("TestResult.html", "out")] string outputFile
            )
        {
            NUnitExecutionReportGenerator generator = new NUnitExecutionReportGenerator();
            generator.GenerateReport();

            string outputFilePath = Path.GetFullPath(outputFile);
            generator.TransformReport(outputFilePath);
        }
    }
}