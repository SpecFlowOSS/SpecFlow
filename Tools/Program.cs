using NConsoler;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.GeneratorTools.BatchTestGeneration;

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

            BatchTestGenerator.ProcessProject(specFlowProject, false, false);
        }
    }
}