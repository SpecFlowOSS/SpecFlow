using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gherkin;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Parser;


namespace TechTalk.SpecFlow.Tracing
{
    internal static class LanguageHelper
    {
        private readonly struct KeywordSet
        {
            public string DefaultKeyword
            {
                get { return Keywords.First(); }
            }
            public readonly string[] Keywords;

            public KeywordSet(string[] keywords)
            {
                this.Keywords = keywords
                    .Select(k => k.Trim())
                    .Where(k => k != "*")
                    .ToArray();
            }
        }

        private class KeywordTranslation : Dictionary<StepDefinitionKeyword, KeywordSet>
        {
            private static readonly Dictionary<StepDefinitionKeyword, Func<GherkinDialect, string[]>> getKeywordTranslations =
                new Dictionary<StepDefinitionKeyword, Func<GherkinDialect, string[]>>
                {
                    {StepDefinitionKeyword.Given, d => d.GivenStepKeywords},
                    {StepDefinitionKeyword.When, d => d.WhenStepKeywords},
                    {StepDefinitionKeyword.Then, d => d.ThenStepKeywords},
                    {StepDefinitionKeyword.And, d => d.AndStepKeywords},
                    {StepDefinitionKeyword.But, d => d.ButStepKeywords},
                };

            public KeywordTranslation(GherkinDialect dialect)
            {
                foreach (StepDefinitionKeyword keyword in EnumHelper.GetValues(typeof(StepDefinitionKeyword)))
                {
                    var keywordList = getKeywordTranslations[keyword](dialect);
                    this[keyword] = new KeywordSet(keywordList);
                }

                DefaultSpecificCulture = CultureInfo.GetCultureInfo(dialect.Language);
            }

            public CultureInfo DefaultSpecificCulture { get; }
        }

        private static readonly Dictionary<CultureInfo, KeywordTranslation> translationCache = new Dictionary<CultureInfo, KeywordTranslation>();

        public static string GetDefaultKeyword(CultureInfo language, StepDefinitionType stepDefinitionType)
        {
            return GetDefaultKeyword(language, stepDefinitionType.ToStepDefinitionKeyword());
        }

        public static string[] GetKeywords(CultureInfo language, StepDefinitionType stepDefinitionType)
        {
            return GetKeywords(language, stepDefinitionType.ToStepDefinitionKeyword());
        }

        public static string[] GetKeywords(CultureInfo language, StepDefinitionKeyword keyword)
        {
            KeywordTranslation translation = GetTranslation(language);
            return translation[keyword].Keywords;
        }

        public static string GetDefaultKeyword(CultureInfo language, StepDefinitionKeyword keyword)
        {
            KeywordTranslation translation = GetTranslation(language);
            return translation[keyword].DefaultKeyword;
        }

        private static KeywordTranslation GetTranslation(CultureInfo language)
        {
            if (!translationCache.TryGetValue(language, out var translation))
            {
                translation = LoadTranslation(language);
            }
            return translation;
        }

        public static CultureInfo GetSpecificCultureInfo(CultureInfo language)
        {
            //HACK: we need to have a better solution
            if (!language.IsNeutralCulture)
                return language;

            KeywordTranslation translation = GetTranslation(language);
            return translation.DefaultSpecificCulture;
        }


        private static KeywordTranslation LoadTranslation(CultureInfo language)
        {
            var dialect = new SpecFlowGherkinDialectProvider(language.Name).DefaultDialect;

            return new KeywordTranslation(dialect);
        }
    }
}
