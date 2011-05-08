namespace TechTalk.SpecFlow.Configuration
{
    public static class ConfigDefaults
    {
        internal const string FeatureLanguage = "en-US";
        internal const string ToolLanguage = "";

        internal const string UnitTestProviderName = "NUnit";

        internal const bool DetectAmbiguousMatches = true;
        internal const bool StopAtFirstError = false;
        internal const MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome = TechTalk.SpecFlow.Configuration.MissingOrPendingStepsOutcome.Inconclusive;

        internal const bool TraceSuccessfulSteps = true;
        internal const bool TraceTimings = false;
        internal const string MinTracedDuration = "0:0:0.1";

        internal const bool AllowDebugGeneratedFiles = false;
        internal const bool AllowRowTests = true;
        internal const string GeneratorPath = null;
    }
}