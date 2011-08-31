namespace TechTalk.SpecFlow.IdeIntegration.Options
{
    public class IntegrationOptions
    {
        public bool EnableSyntaxColoring { get; set; }
        public bool EnableOutlining { get; set; }
        public bool EnableIntelliSense { get; set; }
        public bool EnableAnalysis { get; set; }
        public bool EnableTableAutoFormat { get; set; }
        public bool EnableTracing { get; set; }
        public string TracingCategories { get; set; }
        public TestRunnerTool TestRunnerTool { get; set; }
    }

    public enum TestRunnerTool
    {
        ReSharper,
        MsTest,
        SpecRun
    }
}