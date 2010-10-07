namespace TechTalk.SpecFlow.Bindings
{
    public class StepContext
    {
        public FeatureInfo FeatureInfo { get; private set; }
        public ScenarioInfo ScenarioInfo { get; private set; }

        public StepContext(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            FeatureInfo = featureInfo;
            ScenarioInfo = scenarioInfo;
        }
    }
}