using System;
using System.Collections.Generic;
using System.Globalization;
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
            FeatureTitle = featureInfo?.Title;
            ScenarioTitle = scenarioInfo?.Title; 
            Tags = scenarioInfo != null ? scenarioInfo.CombinedTags : featureInfo != null ? featureInfo.Tags : Array.Empty<string>();
        }
    }
}