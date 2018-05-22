using CommandLine;

namespace TechTalk.SpecFlow.Tools.Options
{
    [Verb("GenerateAll", HelpText = "Generate tests from all feature files in a project.")]
    public class GenerateAllOptions : BaseOptions
    {
        [Option('f', "force",
            HelpText = "Suppresses summary messages.")]
        public bool ForceGeneration { get; set; }

        [Option('v', "verbose",
            HelpText = "Suppresses summary messages.")]
        public bool VerboseOutput { get; set; }

        [Option('d', "debug",
            HelpText = "Used for tool integration.")]
        public bool RequestDebuggerToAttach { get; set; }
    }
}