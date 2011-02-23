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


    }
}