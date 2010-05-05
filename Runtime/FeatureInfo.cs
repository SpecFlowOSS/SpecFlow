using System;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow
{
    public class FeatureInfo
    {
        public string[] Tags { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public CultureInfo Language { get; private set; }
        [Obsolete("Use Language property that is now ensured to be a specific culture.")]
        public CultureInfo CultureInfo
        {
            get { return Language; }
        }

        public FeatureInfo(CultureInfo language, string title, string description, params string[] tags)
        {
            if (language.IsNeutralCulture)
            {
                // for backwards compatibility (execution of files that were generated with pre 1.3)
                language = LanguageHelper.GetSpecificCultureInfo(language);
            }

            Language = language;
            Title = title;
            Description = description;
            Tags = tags;
        }
    }
}