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
            Tag = RemoveLeadingAt(tag);
            FeatureTitle = featureTitle;
            ScenarioTitle = scenarioTitle;
        }

        private string RemoveLeadingAt(string tag)
        {
            if (tag == null || !tag.StartsWith("@"))
                return tag;

            return tag.Substring(1); // remove leading "@"
        }

        public bool Match(StepContext stepContext)
        {
            int dummy;
            return Match(stepContext, out dummy);
        }

        private readonly string[] emptyTagList = new string[0];
        public bool Match(StepContext stepContext, out int scopeMatches)
        {
            scopeMatches = 0;
            var tags = stepContext.Tags ?? emptyTagList;
            if (Tag != null)
            {
                if (!tags.Contains(Tag))
                    return false;

                scopeMatches++;
            }
            if (FeatureTitle != null)
            {
                if (!string.Equals(FeatureTitle, stepContext.FeatureTitle, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                scopeMatches++;
            }
            if (ScenarioTitle != null)
            {
                if (!string.Equals(ScenarioTitle, stepContext.ScenarioTitle, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                scopeMatches++;
            }

            return true;
        }
    }
}