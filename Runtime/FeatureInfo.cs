using System;
using System.Globalization;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow
{
    public class FeatureInfo
    {
        public string[] Tags { get; private set; }
        public ProgrammingLanguage GenerationTargetLanguage { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public CultureInfo Language { get; private set; }

        public FeatureInfo(CultureInfo language, string title, string description, params string[] tags)
            : this(language, title, description, ProgrammingLanguage.CSharp, tags)
        {
        }

        public FeatureInfo(CultureInfo language, string title, string description, ProgrammingLanguage programmingLanguage, params string[] tags)
        {
            if (language.IsNeutralCulture)
            {
                // for backwards compatibility (execution of files that were generated with pre 1.3)
                language = LanguageHelper.GetSpecificCultureInfo(language);
            }

            Language = language;
            Title = title;
            Description = description;
            GenerationTargetLanguage = programmingLanguage;
            Tags = tags ?? new string[0];
        }
    }
}