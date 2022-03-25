namespace TechTalk.SpecFlow.Generator.Generation
{
    public class GeneratorConstants
    {
        public const string DEFAULT_NAMESPACE = "SpecFlowTests";
        public const string TEST_NAME_FORMAT = "{0}";
        public const string SCENARIO_INITIALIZE_NAME = "ScenarioInitialize";
        public const string SCENARIO_START_NAME = "ScenarioStartAsync";
        public const string SCENARIO_CLEANUP_NAME = "ScenarioCleanupAsync";
        public const string TEST_INITIALIZE_NAME = "TestInitializeAsync";
        public const string TEST_CLEANUP_NAME = "TestTearDownAsync";
        public const string TESTCLASS_INITIALIZE_NAME = "FeatureSetupAsync";
        public const string TESTCLASS_CLEANUP_NAME = "FeatureTearDownAsync";
        public const string BACKGROUND_NAME = "FeatureBackgroundAsync";
        public const string TESTRUNNER_FIELD = "testRunner";
        public const string SPECFLOW_NAMESPACE = "TechTalk.SpecFlow";
        public const string SCENARIO_OUTLINE_EXAMPLE_TAGS_PARAMETER = "exampleTags";
        public const string SCENARIO_TAGS_VARIABLE_NAME = "tagsOfScenario";
        public const string SCENARIO_ARGUMENTS_VARIABLE_NAME = "argumentsOfScenario";
        public const string FEATURE_TAGS_VARIABLE_NAME = "featureTags";
    }
}