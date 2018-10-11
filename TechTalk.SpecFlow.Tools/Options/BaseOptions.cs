using CommandLine;

namespace TechTalk.SpecFlow.Tools.Options
{
    public class BaseOptions
    {
        [Option('p', "ProjectFile",
            Required = true,
            SetName = "byprojfile",
            HelpText = "Visual Studio Project File containing specs.")]
        public string ProjectFile { get; set; }
    }
}
