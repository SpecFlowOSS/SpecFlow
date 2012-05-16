using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Bindings
{
    public class StepContext
    {
        public string FeatureTitle { get; private set; }
        public string ScenarioTitle { get; private set; }
        public CultureInfo Language { get; private set; }

        public IEnumerable<string> Tags { get; private set; }

        public StepContext(string featureTitle, string scenarioTitle, IEnumerable<string> tags, CultureInfo language)
        {
            FeatureTitle = featureTitle;
            ScenarioTitle = scenarioTitle;
            Tags = tags;
            Language = language;
        }

        public StepContext(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            Language = featureInfo == null ? CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage) : featureInfo.Language;
            FeatureTitle = featureInfo == null ? null : featureInfo.Title;
            ScenarioTitle = scenarioInfo == null ? null : scenarioInfo.Title; 

            var tags = Enumerable.Empty<string>();
            if (featureInfo != null && featureInfo.Tags != null)
                tags = tags.Concat(featureInfo.Tags);
            if (scenarioInfo != null && scenarioInfo.Tags != null)
                tags = tags.Concat(scenarioInfo.Tags).Distinct();
            Tags = tags;
        }
    }
}