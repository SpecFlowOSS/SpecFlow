using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Reporting.MsTestExecutionReport;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport;
using TechTalk.SpecFlow.Reporting.StepDefinitionReport;
using TechTalk.SpecFlow.Tools.Options;
using TechTalk.SpecFlow.Tracing;
using MsBuildProjectReader = TechTalk.SpecFlow.Generator.Project.MsBuildProjectReader;
using TextWriterTraceListener = TechTalk.SpecFlow.Tracing.TextWriterTraceListener;

namespace TechTalk.SpecFlow.Tools
{
    public class Program
    {
        private const int ERROR_BAD_ARGUMENTS = 0xA0;

        private static void Main(string[] args)
        {
            var parser = new CommandLine.Parser(with =>
            {
                with.CaseSensitive = false;
                with.CaseInsensitiveEnumValues = true;
                with.HelpWriter = Console.Error;
                with.EnableDashDash = true;
            });

            parser.ParseArguments<GenerateAllOptions, StepDefinitionOptions, NUnitOptions, MsTestOptions>(args)
                .WithParsed<GenerateAllOptions>(GenerateAll)
                .WithParsed<StepDefinitionOptions>(StepDefinitionReport)
                .WithParsed<NUnitOptions>(NUnitExecutionReport)
                .WithParsed<MsTestOptions>(MsTestExecutionReport)
                .WithNotParsed(err => Environment.ExitCode = ERROR_BAD_ARGUMENTS);
        }

        public static void GenerateAll(GenerateAllOptions parameters)
        {
            if (parameters.RequestDebuggerToAttach)
                Debugger.Launch();

            ITraceListener traceListener = parameters.VerboseOutput ? (ITraceListener)new TextWriterTraceListener(Console.Out) : new NullListener();

            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(parameters.ProjectFile));
            var batchGenerator = new BatchGenerator(traceListener, new TestGeneratorFactory());

            batchGenerator.OnError += BatchGenerator_OnError;

            batchGenerator.ProcessProject(specFlowProject, parameters.ForceGeneration);
        }

        static void BatchGenerator_OnError(Generator.Interfaces.FeatureFileInput featureFileInput, Generator.Interfaces.TestGeneratorResult testGeneratorResult)
        {
            Console.Error.WriteLine("Error file {0}", featureFileInput.ProjectRelativePath);
            Console.Error.WriteLine(String.Join(Environment.NewLine, testGeneratorResult.Errors.Select(e => String.Format("Line {0}:{1} - {2}", e.Line, e.LinePosition, e.Message))));
        }

        #region Reports

        public static void StepDefinitionReport(StepDefinitionOptions parameters)
        {
            if (parameters.RequestDebuggerToAttach)
                Debugger.Launch();

            StepDefinitionReportParameters reportParameters =
                new StepDefinitionReportParameters(parameters.ProjectFile, parameters.OutputFile, parameters.XsltFile, parameters.BinFolder, true);
            var generator = new StepDefinitionReportGenerator(reportParameters);
            generator.GenerateAndTransformReport();
        }

        public static void NUnitExecutionReport(NUnitOptions parameters)
        {
            NUnitExecutionReportParameters reportParameters;

            if (parameters.ProjectFile == null)
            {
                reportParameters = new NUnitExecutionReportParameters(parameters.ProjectName, parameters.FeatureLanguage, parameters.XmlTestResult, parameters.TestOutput, parameters.OutputFile, parameters.XsltFile);
            }
            else
            {
                reportParameters = new NUnitExecutionReportParameters(parameters.ProjectFile, parameters.XmlTestResult, parameters.TestOutput, parameters.OutputFile, parameters.XsltFile);
            }

            var generator = new NUnitExecutionReportGenerator(reportParameters);
            generator.GenerateAndTransformReport();
        }

        public static void MsTestExecutionReport(MsTestOptions parameters)
        {
            MsTestExecutionReportParameters reportParameters;

            if (parameters.ProjectFile == null)
            {
                reportParameters = new MsTestExecutionReportParameters(parameters.ProjectName, parameters.FeatureLanguage, parameters.TestResult, parameters.OutputFile, parameters.XsltFile);
            }
            else
            {
                reportParameters = new MsTestExecutionReportParameters(parameters.ProjectFile, parameters.TestResult, parameters.OutputFile, parameters.XsltFile);
            }

            var generator = new MsTestExecutionReportGenerator(reportParameters);
            generator.GenerateAndTransformReport();
        }
        #endregion
    }
}