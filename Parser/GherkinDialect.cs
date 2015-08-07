using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Gherkin3GherkinDialect=Gherkin.GherkinDialect;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Parser
{
    public class GherkinDialect
    {
        internal Gherkin3GherkinDialect NativeLanguageService { get; private set; }
        internal LanguageInfo LanguageInfo { get; private set; }

        public CultureInfo CultureInfo
        {
            get { return LanguageInfo.CultureInfo; }
        }

        internal GherkinDialect(LanguageInfo languageInfo, Gherkin3GherkinDialect nativeLanguageService)
        {
            NativeLanguageService = nativeLanguageService;
            LanguageInfo = languageInfo;
        }

        public override bool Equals(object obj)
        {
            var other = obj as GherkinDialect;

            return other != null && other.LanguageInfo.Equals(LanguageInfo);
        }

        public override int GetHashCode()
        {
            return LanguageInfo.GetHashCode();
        }

        public bool IsStepKeyword(string keyword)
        {
            return TryParseStepKeyword(keyword) != null;
        }

        public StepKeyword? TryParseStepKeyword(string keyword)
        {
            if (NativeLanguageService.AndStepKeywords.Contains(keyword))
                return StepKeyword.And;
            // this is checked at the first place to interpret "*" as "and"

            if (NativeLanguageService.GivenStepKeywords.Contains(keyword))
                return StepKeyword.Given;

            if (NativeLanguageService.WhenStepKeywords.Contains(keyword))
                return StepKeyword.When;

            if (NativeLanguageService.ThenStepKeywords.Contains(keyword))
                return StepKeyword.Then;

            if (NativeLanguageService.ButStepKeywords.Contains(keyword))
                return StepKeyword.But;

            // In Gherkin, the space at the end is also part of the keyword, becase in some 
            // languages, there is no space between the step keyword and the step text.
            // To support the keywords without leading space as well, we retry the matching with 
            // an additional space too.
            if (!keyword.EndsWith(" "))
                return TryParseStepKeyword(keyword + " ");

            return null;
        }

        public IEnumerable<string> GetKeywords()
        {
            return GetStepKeywords().Concat(GetBlockKeywords()).OrderBy(k => k);
        }

        public IEnumerable<string> GetBlockKeywords()
        {
            var keywords = Enum.GetValues(typeof(GherkinBlockKeyword)).Cast<GherkinBlockKeyword>().Aggregate(Enumerable.Empty<string>(),
                (current, stepKeyword) => current.Concat(GetBlockKeywords(stepKeyword)));
            return keywords.Distinct().OrderBy(k => k);
        }

        public IEnumerable<string> GetStepKeywords()
        {
            var keywords = Enum.GetValues(typeof(StepKeyword)).Cast<StepKeyword>().Aggregate(Enumerable.Empty<string>(), 
                (current, stepKeyword) => current.Concat(GetStepKeywords(stepKeyword)));
            return keywords.Distinct().OrderBy(k => k);
        }

        public IEnumerable<string> GetStepKeywords(StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    return NativeLanguageService.GivenStepKeywords;
                case StepKeyword.When:
                    return NativeLanguageService.WhenStepKeywords;
                case StepKeyword.Then:
                    return NativeLanguageService.ThenStepKeywords;
                case StepKeyword.And:
                    return NativeLanguageService.AndStepKeywords;
                case StepKeyword.But:
                    return NativeLanguageService.ButStepKeywords;
                default:
                    throw new NotSupportedException();
            }
        }

        public IEnumerable<string> GetBlockKeywords(GherkinBlockKeyword blockKeyword)
        {
            switch (blockKeyword)
            {
                case GherkinBlockKeyword.Feature:
                    return NativeLanguageService.FeatureKeywords;
                case GherkinBlockKeyword.Background:
                    return NativeLanguageService.ButStepKeywords;
                case GherkinBlockKeyword.Scenario:
                    return NativeLanguageService.ScenarioKeywords;
                case GherkinBlockKeyword.ScenarioOutline:
                    return NativeLanguageService.ScenarioOutlineKeywords;
                case GherkinBlockKeyword.Examples:
                    return NativeLanguageService.ExamplesKeywords;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}