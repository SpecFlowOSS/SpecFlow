using System;
using NConsoler;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
            return;
        }

        [Action("Generate tests from all feature files")]
        public static void Generate(
            [Required] string projectFile,
            [Optional(false, "force", "f")] bool forceGeneration,
            [Optional(false, "verbose", "v")] bool verboseOutput
            )
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);

            BatchGenerator batchGenerator = new BatchGenerator(Console.Out, verboseOutput);
            batchGenerator.ProcessProject(specFlowProject, forceGeneration);
        }

        [Action]
        public static void ToBeDefinedAction()
        {
            
        }
    }
}