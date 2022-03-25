using System;
using System.Linq;

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

            var tags = stepContext.Tags;

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

        protected bool Equals(BindingScope other)
        {
            return string.Equals(Tag, other.Tag) && string.Equals(FeatureTitle, other.FeatureTitle) && string.Equals(ScenarioTitle, other.ScenarioTitle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BindingScope) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FeatureTitle != null ? FeatureTitle.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ScenarioTitle != null ? ScenarioTitle.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
