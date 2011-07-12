using System;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingScopeNew
    {
        public string Tag { get; private set; }
        public string FeatureTitle { get; private set; }
        public string ScenarioTitle { get; private set; }

        public BindingScopeNew(string tag, string featureTitle, string scenarioTitle)
        {
            Tag = tag;
            FeatureTitle = featureTitle;
            ScenarioTitle = scenarioTitle;
        }

        public bool Match(StepScopeNew stepScope)
        {
            int dummy;
            return Match(stepScope, out dummy);
        }

        private readonly string[] emptyTagList = new string[0];
        public bool Match(StepScopeNew stepScope, out int scopeMatches)
        {
            scopeMatches = 0;
            var tags = stepScope.Tags ?? emptyTagList;
            if (Tag != null)
            {
                if (!tags.Contains(Tag))
                    return false;

                scopeMatches++;
            }
            if (FeatureTitle != null)
            {
                if (!string.Equals(FeatureTitle, stepScope.FeatureTitle, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                scopeMatches++;
            }
            if (ScenarioTitle != null)
            {
                if (!string.Equals(ScenarioTitle, stepScope.ScenarioTitle, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                scopeMatches++;
            }

            return true;
        }
    }
}