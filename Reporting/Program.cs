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
            [Optional("StepDefinitionReport.html", "out")] string outputFile,
            [Optional(null, "features")] string[] featureFiles
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            bool onlySelectedFeatures = featureFiles != null && featureFiles.Length > 0;

            List<Feature> parsedFeatures = onlySelectedFeatures ?
                ParserHelper.GetParsedFeatures(featureFiles) :
                ParserHelper.GetParsedFeatures(specFlowProject);

            var basePath = Path.Combine(specFlowProject.ProjectFolder, binFolder);
            List<BindingInfo> bindings = BindingCollector.CollectBindings(specFlowProject, basePath);

            StepDefinitionReportGenerator generator = new StepDefinitionReportGenerator(specFlowProject, bindings, parsedFeatures, 
                !onlySelectedFeatures, onlySelectedFeatures);
            generator.GenerateReport();

            string outputFilePath = Path.Combine(basePath, outputFile);
            generator.TransformReport(outputFilePath);
        }

        [Action]
        public static void NUnitExecutionReport(
            [Required] string projectFile,
            [Optional("TestResult.xml")] string xmlTestResult,
            [Optional("TestResult.txt", "testOutput")] string labeledTestOutput,
            [Optional("TestResult.html", "out")] string outputFile
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            NUnitExecutionReportGenerator generator = new NUnitExecutionReportGenerator(
                specFlowProject, 
                Path.GetFullPath(xmlTestResult),
                Path.GetFullPath(labeledTestOutput));
            generator.GenerateReport();

            string outputFilePath = Path.GetFullPath(outputFile);
            generator.TransformReport(outputFilePath);
        }
    }
}