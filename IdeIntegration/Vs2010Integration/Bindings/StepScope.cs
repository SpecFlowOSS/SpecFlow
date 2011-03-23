using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepScope
    {
        public string FeatureTitle { get; private set; }
        public string ScenarioTitle { get; private set; }
        public IEnumerable<string> Tags { get; private set; }

        public StepScope(string featureTitle, string scenarioTitle, IEnumerable<string> tags)
        {
            FeatureTitle = featureTitle;
            ScenarioTitle = scenarioTitle;
            Tags = tags;
        }
    }
}