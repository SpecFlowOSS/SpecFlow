using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Bindings
{
    public class BindingScope
    {
        public string Tag { get; private set; }
        public string FeatureTitle { get; private set; }
        public string ScenarioTitle { get; private set; }

        public BindingScope(string tag, string featureTitle, string scenarioTitle)
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

        public bool Match(StepContext stepContext, out int scopeMatches)
        {
            scopeMatches = 0;

            var tags = Enumerable.Empty<string>();
            if (stepContext.FeatureInfo != null && stepContext.FeatureInfo.Tags != null)
                tags = tags.Concat(stepContext.FeatureInfo.Tags);
            if (stepContext.ScenarioInfo != null && stepContext.ScenarioInfo.Tags != null)
                tags = tags.Concat(stepContext.ScenarioInfo.Tags);

            if (Tag != null)
            {
                if (!tags.Contains(Tag))
                    return false;

                scopeMatches++;
            }
            if (FeatureTitle != null && stepContext.FeatureInfo != null)
            {
                if (!string.Equals(FeatureTitle, stepContext.FeatureInfo.Title, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                scopeMatches++;
            }
            if (ScenarioTitle != null && stepContext.ScenarioInfo != null)
            {
                if (!string.Equals(ScenarioTitle, stepContext.ScenarioInfo.Title, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                scopeMatches++;
            }

            return true;
        }
    }
}
