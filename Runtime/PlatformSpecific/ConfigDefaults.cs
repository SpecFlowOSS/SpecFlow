namespace TechTalk.SpecFlow.Configuration
{
    public static class ConfigDefaults
    {
        public const string FeatureLanguage = "en-US";
        public const string ToolLanguage = "";

        public const string UnitTestProviderName = "NUnit";

        public const bool DetectAmbiguousMatches = true;
        public const bool StopAtFirstError = false;
        public const MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome = TechTalk.SpecFlow.Configuration.MissingOrPendingStepsOutcome.Pending;

        public const bool TraceSuccessfulSteps = true;
        public const bool TraceTimings = false;
        public const string MinTracedDuration = "0:0:0.1";

        public const bool AllowDebugGeneratedFiles = false;
        public const bool AllowRowTests = true;
        public const bool GenerateAsyncTests = false;
        public const string GeneratorPath = null;
    }
}