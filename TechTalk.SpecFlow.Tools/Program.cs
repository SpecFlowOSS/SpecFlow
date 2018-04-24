using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NConsoler;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;
using MsBuildProjectReader = TechTalk.SpecFlow.Generator.Project.MsBuildProjectReader;
using TextWriterTraceListener = TechTalk.SpecFlow.Tracing.TextWriterTraceListener;

namespace TechTalk.SpecFlow.Tools
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
        }

        [Action("Generate tests from all feature files in a project")]
        public static void GenerateAll(
            [Required(Description = "Visual Studio Project File containing features")] string projectFile,
            [Optional(false, "force", "f")] bool forceGeneration,
            [Optional(false, "verbose", "v")] bool verboseOutput,
            [Optional(false, "debug", Description = "Used for tool integration")] bool requestDebuggerToAttach)
        {
            if (requestDebuggerToAttach)
                Debugger.Launch();

            ITraceListener traceListener = verboseOutput ? (ITraceListener)new TextWriterTraceListener(Console.Out) : new NullListener();

            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(projectFile));
            var batchGenerator = new BatchGenerator(traceListener, new TestGeneratorFactory());

            batchGenerator.OnError += batchGenerator_OnError;

            batchGenerator.ProcessProject(specFlowProject, forceGeneration);
        }

        private static void batchGenerator_OnError(Generator.Interfaces.FeatureFileInput featureFileInput, Generator.Interfaces.TestGeneratorResult testGeneratorResult)
        {
            Console.Error.WriteLine("Error file {0}", featureFileInput.ProjectRelativePath);
            Console.Error.WriteLine(string.Join(Environment.NewLine, testGeneratorResult.Errors.Select(e => $"Line {e.Line}:{e.LinePosition} - {e.Message}")));
        }
    }
}