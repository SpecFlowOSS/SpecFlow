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
            if (featureInfo is not null)
            {
                Language = featureInfo.Language;
                FeatureTitle = featureInfo.Title;

                if (scenarioInfo is not null)
                {
                    ScenarioTitle = scenarioInfo.Title;

                    string[] featureTags = featureInfo.Tags;
                    string[] scenarioTags = scenarioInfo.Tags;
                    if (featureTags is null || featureTags.Length == 0)
                    {
                        Tags = scenarioTags ?? Array.Empty<string>();
                    }
                    else
                    {
                        if (scenarioTags is null || scenarioTags.Length == 0)
                        {
                            Tags = featureTags;
                        }
                        else
                        {
                            Tags = CombineTags(featureInfo.Tags, scenarioInfo.Tags);
                        }
                    }
                }
                else
                {
                    Tags = featureInfo.Tags ?? Array.Empty<string>();
                }
            }
            else
            {
                Language = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
                if (scenarioInfo is null)
                {
                    Tags = Array.Empty<string>();
                }
                else
                {
                    ScenarioTitle = scenarioInfo.Title;
                    Tags = scenarioInfo.Tags ?? Array.Empty<string>();
                }
            }
        }

        private static IEnumerable<string> CombineTags(string[] featureTags, string[] scenarioTags)
        {
            var result = new HashSet<string>(featureTags);
            foreach (string tag in scenarioTags)
            {
                result.Add(tag);
            }

            return result;
        }
    }
}