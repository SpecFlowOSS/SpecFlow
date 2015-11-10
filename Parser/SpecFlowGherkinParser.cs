using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser.Compatibility;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowGherkinParser
    {
        private readonly GherkinDialectProvider dialectProvider;

        private class SpecFlowGherkinDialectProvider : GherkinDialectProvider
        {
            public SpecFlowGherkinDialectProvider(string defaultLanguage) : base(defaultLanguage)
            {
            }

            public override GherkinDialect GetDialect(string language, Location location)
            {
                location = location ?? new Location(); //TODO: fix in gherkin3, GetDialect needs a location for throwing NoSuchLanguageException

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
                        return new GherkinDialect(language, languageBaseDialect.FeatureKeywords, languageBaseDialect.BackgroundKeywords, languageBaseDialect.ScenarioKeywords, languageBaseDialect.ScenarioOutlineKeywords, languageBaseDialect.ExamplesKeywords, languageBaseDialect.GivenStepKeywords, languageBaseDialect.WhenStepKeywords, languageBaseDialect.ThenStepKeywords, languageBaseDialect.AndStepKeywords, languageBaseDialect.ButStepKeywords);
                    }
                }

                return base.GetDialect(language, location);
            }
        }

        internal GherkinDialectProvider DialectProvider
        {
            get { return dialectProvider; }
        }

        public SpecFlowGherkinParser(CultureInfo defaultLanguage)
        {
            dialectProvider = new SpecFlowGherkinDialectProvider(defaultLanguage.Name);
        }

        private static StepKeyword GetStepKeyword(GherkinDialect dialect, string stepKeyword)
        {
            if (dialect.AndStepKeywords.Contains(stepKeyword)) // we need to check "And" first, as the '*' is also part of the Given, When and Then keywords
                return StepKeyword.And;
            if (dialect.GivenStepKeywords.Contains(stepKeyword))
                return StepKeyword.Given;
            if (dialect.WhenStepKeywords.Contains(stepKeyword))
                return StepKeyword.When;
            if (dialect.ThenStepKeywords.Contains(stepKeyword))
                return StepKeyword.Then;
            if (dialect.ButStepKeywords.Contains(stepKeyword))
                return StepKeyword.But;

            return StepKeyword.And;
        }

        private class SpecFlowAstBuilder : PatchedAstBuilder<SpecFlowFeature>
        {
            private readonly string sourceFilePath;
            private ScenarioBlock scenarioBlock = ScenarioBlock.Given;

            public SpecFlowAstBuilder(string sourceFilePath)
            {
                this.sourceFilePath = sourceFilePath;
            }

            protected override Feature CreateFeature(Tag[] tags, Location location, string language, string keyword, string name, string description,
                Background background, ScenarioDefinition[] scenariodefinitions, Comment[] comments)
            {
                return new SpecFlowFeature(tags, location, language, keyword, name, description, background, scenariodefinitions, comments, sourceFilePath);
            }

            protected override Step CreateStep(Location location, string keyword, string text, StepArgument argument, AstNode node)
            {
                var token = node.GetToken(TokenType.StepLine);
                var stepKeyword = GetStepKeyword(token.MatchedGherkinDialect, keyword);
                scenarioBlock = stepKeyword.ToScenarioBlock() ?? scenarioBlock;

                return new SpecFlowStep(location, keyword, text, argument, stepKeyword, scenarioBlock);
            }

            private void ResetBlock()
            {
                scenarioBlock = ScenarioBlock.Given;
            }

            protected override Scenario CreateScenario(Tag[] tags, Location location, string keyword, string name, string description, Step[] steps)
            {
                ResetBlock();
                return base.CreateScenario(tags, location, keyword, name, description, steps);
            }

            protected override ScenarioOutline CreateScenarioOutline(Tag[] tags, Location location, string keyword, string name, string description, Step[] steps, Examples[] examples)
            {
                ResetBlock();
                return base.CreateScenarioOutline(tags, location, keyword, name, description, steps, examples);
            }

            protected override Background CreateBackground(Location location, string keyword, string name, string description, Step[] steps)
            {
                ResetBlock();
                return base.CreateBackground(location, keyword, name, description, steps);
            }
        }

        public SpecFlowFeature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var parser = new Parser<SpecFlowFeature>(new SpecFlowAstBuilder(sourceFilePath));
            var tokenMatcher = new TokenMatcher(dialectProvider);
            var feature = parser.Parse(new TokenScanner(featureFileReader), tokenMatcher);

            CheckSemanticErrors(feature);

            return feature;
        }

        private void CheckSemanticErrors(Feature feature)
        {
            var errors = new List<ParserException>();

            // duplicate scenario name
            var duplicatedScenarios = feature.ScenarioDefinitions.GroupBy(sd => sd.Name, sd => sd).Where(g => g.Count() > 1).ToArray();
            errors.AddRange(
                duplicatedScenarios.Select(g =>
                    new SemanticParserException(
                        string.Format("Feature file already contains a scenario with name '{0}'", g.Key),
                        g.ElementAt(1).Location)));

            // collect
            if (errors.Count == 1)
                throw errors[0];
            if (errors.Count > 1)
                throw new CompositeParserException(errors.ToArray());
        }
    }
}
