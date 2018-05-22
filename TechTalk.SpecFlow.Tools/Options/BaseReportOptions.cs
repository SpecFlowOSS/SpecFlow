using CommandLine;

namespace TechTalk.SpecFlow.Tools.Options
{
    public class BaseReportOptions : BaseOptions
    {
        [Option('n', "ProjectName",
            Required = true,
            SetName = "byprojname",
            HelpText = "Help Project Name")]
        public string ProjectName { get; set; }

        [Option('l', "FeatureLanguage",
            Default = "en-US",
            SetName = "byprojname",
            HelpText = "Help Feature Language")]
        public string FeatureLanguage { get; set; }

        [Option('x', "XsltFile",
             HelpText = "Xslt file to use, defaults to built-in stylesheet if not provided.")]
        public string XsltFile { get; set; }

        [Option('o', "OutputFile",
            Default = "TestResult.html",
            HelpText = "Generated Output File. Defaults to TestResult.html")]
        public string OutputFile { get; set; }
    }
}