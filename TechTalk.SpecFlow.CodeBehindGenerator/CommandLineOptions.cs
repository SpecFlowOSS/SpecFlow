using CommandLine;

namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    class CommandLineOptions
    {
        [Option]
        public int Port { get; set; }

        [Option]
        public bool Debug { get; set; }
    }
}
