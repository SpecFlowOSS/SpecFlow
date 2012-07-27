using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.Configuration
{
    public static class ConfigDefaults
    {
        internal const string FeatureLanguage = "en-US";
        internal const string ToolLanguage = "";

        internal const string UnitTestProviderName = "MSTest.Silverlight3";

        internal const bool DetectAmbiguousMatches = true;
        internal const bool StopAtFirstError = false;
        internal const MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome = TechTalk.SpecFlow.Configuration.MissingOrPendingStepsOutcome.Inconclusive;

        internal const bool TraceSuccessfulSteps = true;
        internal const bool TraceTimings = false;
        internal const string MinTracedDuration = "0:0:0.1";
        internal const StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle = TechTalk.SpecFlow.BindingSkeletons.StepDefinitionSkeletonStyle.RegexAttribute;

        internal const bool AllowDebugGeneratedFiles = false;
    }
}