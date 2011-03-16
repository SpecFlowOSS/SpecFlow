using System.Linq;
using System.Collections.Generic;
using gherkin;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Parser
{
    public class GherkinDialect
    {
        internal I18n NativeLanguageService { get; private set; }
        internal LanguageInfo LanguageInfo { get; private set; }

        internal GherkinDialect(LanguageInfo languageInfo, I18n nativeLanguageService)
        {
            NativeLanguageService = nativeLanguageService;
            LanguageInfo = languageInfo;
        }

        public override bool Equals(object obj)
        {
            GherkinDialect other = obj as GherkinDialect;

            return other != null && other.LanguageInfo.Equals(LanguageInfo);
        }

        public override int GetHashCode()
        {
            return LanguageInfo.GetHashCode();
        }

        public bool IsStepKeyword(string keyword)
        {
            return GetStepKeyword(keyword) != null;
        }

        public StepKeyword? GetStepKeyword(string keyword)
        {
            if (NativeLanguageService.keywords("and").contains(keyword))
                return StepKeyword.And;
            // this is checked at the first place to interpret "*" as "and"

            if (NativeLanguageService.keywords("given").contains(keyword))
                return StepKeyword.Given;

            if (NativeLanguageService.keywords("when").contains(keyword))
                return StepKeyword.When;

            if (NativeLanguageService.keywords("then").contains(keyword))
                return StepKeyword.Then;

            if (NativeLanguageService.keywords("but").contains(keyword))
                return StepKeyword.But;

            // In Gherkin, the space at the end is also part of the keyword, becase in some 
            // languages, there is no space between the step keyword and the step text.
            // To support the keywords without leading space as well, we retry the matching with 
            // an additional space too.
            if (!keyword.EndsWith(" "))
                return GetStepKeyword(keyword + " ");

            return null;
        }

        public IEnumerable<string> GetKeywords()
        {
            return GetStepKeywords().Concat(GetBlockKeywords()).OrderBy(k => k);
        }

        public IEnumerable<string> GetBlockKeywords()
        {
            var keywords = Enumerable.Empty<string>();
            keywords = keywords.Concat(NativeLanguageService.keywords("feature").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("background").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("scenario").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("scenario_outline").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("examples").toArray().Cast<string>());
            return keywords.Distinct().OrderBy(k => k);
        }

        public IEnumerable<string> GetStepKeywords()
        {
            var keywords = Enumerable.Empty<string>();
            keywords = keywords.Concat(NativeLanguageService.keywords("given").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("when").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("then").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("and").toArray().Cast<string>());
            keywords = keywords.Concat(NativeLanguageService.keywords("but").toArray().Cast<string>());
            return keywords.Distinct().OrderBy(k => k);
        }


    }
}