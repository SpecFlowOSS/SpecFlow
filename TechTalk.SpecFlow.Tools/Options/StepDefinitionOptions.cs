using CommandLine;

namespace TechTalk.SpecFlow.Tools.Options
{
    [Verb("StepDefinitionReport", HelpText = "Generates a report about usage and binding of steps.")]
    public class StepDefinitionOptions : BaseOptions
    {
        [Option('x', "XsltFile",
            HelpText = "Xslt file to use, defaults to built-in stylesheet if not provided.")]
        public string XsltFile { get; set; }

        [Option('b', "binFolder",
            Default = @"bin\Debug",
            HelpText = @"Path for Spec dll e.g. Company.Specs.dll. Defaults to bin\Debug.")]
        public string BinFolder { get; set; }

        [Option('o', "OutputFile",
            Default = "TestResult.html",
            HelpText = "Generated Output File. Defaults to TestResult.html")]
        public string OutputFile { get; set; }

        [Option('d', "debug",
            HelpText = "Used for tool integration.")]
        public bool RequestDebuggerToAttach { get; set; }
    }
}