using Gherkin;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowGherkinDialectProvider : GherkinDialectProvider
    {
        public SpecFlowGherkinDialectProvider(string defaultLanguage) : base(defaultLanguage)
        {
        }

        public override GherkinDialect GetDialect(string language, Location location)
        {
            if (language.Contains("-"))
            {
                try
                {
                    return base.GetDialect(language, location);
                }
                catch (NoSuchLanguageException)
                {
                    var languageBase = language.Split('-')[0];
                    var languageBaseDialect = base.GetDialect(languageBase, location);
                    return new GherkinDialect(language, languageBaseDialect.FeatureKeywords, languageBaseDialect.RuleKeywords, languageBaseDialect.BackgroundKeywords, languageBaseDialect.ScenarioKeywords, languageBaseDialect.ScenarioOutlineKeywords, languageBaseDialect.ExamplesKeywords, languageBaseDialect.GivenStepKeywords, languageBaseDialect.WhenStepKeywords, languageBaseDialect.ThenStepKeywords, languageBaseDialect.AndStepKeywords, languageBaseDialect.ButStepKeywords);
                }
            }

            return base.GetDialect(language, location);
        }
    }
}