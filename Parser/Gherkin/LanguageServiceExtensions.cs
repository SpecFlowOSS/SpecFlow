using System;
using System.Linq;
using gherkin;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public static class LanguageServiceExtensions
    {
        public static bool IsStepKeyword(this I18n languageService, string keyword)
        {
            return GetStepKeyword(languageService, keyword) != null;
        }

        public static StepKeyword? GetStepKeyword(this I18n languageService, string keyword)
        {
            if (languageService.keywords("and").contains(keyword))
                return StepKeyword.And;
            // this is checked at the first place to interpret "*" as "and"

            if (languageService.keywords("given").contains(keyword))
                return StepKeyword.Given;

            if (languageService.keywords("when").contains(keyword))
                return StepKeyword.When;

            if (languageService.keywords("then").contains(keyword))
                return StepKeyword.Then;

            if (languageService.keywords("but").contains(keyword))
                return StepKeyword.But;

            // In Gherkin, the space at the end is also part of the keyword, becase in some 
            // languages, there is no space between the step keyword and the step text.
            // To support the keywords without leading space as well, we retry the matching with 
            // an additional space too.
            if (!keyword.EndsWith(" "))
                return GetStepKeyword(languageService, keyword + " ");

            return null; 
        }

        public static ScenarioBlock? ToScenarioBlock(this StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    return ScenarioBlock.Given;
                case StepKeyword.When:
                    return ScenarioBlock.When;
                case StepKeyword.Then:
                    return ScenarioBlock.Then;
            }
            return null;
        }
    }
}
