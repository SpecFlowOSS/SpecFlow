using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepContext
    {
        public FeatureInfo FeatureInfo { get; private set; }
        public ScenarioInfo ScenarioInfo { get; private set; }

        public IEnumerable<string> Tags { get { return FeatureInfo.Tags.Concat(ScenarioInfo.Tags).Distinct(); } }

        public StepContext(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            FeatureInfo = featureInfo;
            ScenarioInfo = scenarioInfo;
        }
    }
}