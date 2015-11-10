using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Gherkin;
using Gherkin.Ast;

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

        private class SpecFlowAstBuilder : IAstBuilder<SpecFlowFeature>
        {
            //TODO: derive from AstBuilder<SpecFlowFeature> and override CreateFeature when Gherkin v3.1.3 is released
            private readonly AstBuilder<Feature> baseAstBuilder = new AstBuilder<Feature>();
            private readonly string sourceFilePath;

            public SpecFlowAstBuilder(string sourceFilePath)
            {
                this.sourceFilePath = sourceFilePath;
            }

            public void Reset()
            {
                baseAstBuilder.Reset();
            }

            public void Build(Token token)
            {
                baseAstBuilder.Build(token);
            }

            public void StartRule(RuleType ruleType)
            {
                baseAstBuilder.StartRule(ruleType);
            }

            public void EndRule(RuleType ruleType)
            {
                baseAstBuilder.EndRule(ruleType);
            }

            public SpecFlowFeature GetResult()
            {
                var baseFeature = baseAstBuilder.GetResult();
                return new SpecFlowFeature(baseFeature.Tags.ToArray(), baseFeature.Location, baseFeature.Language, baseFeature.Keyword, baseFeature.Name, baseFeature.Description, baseFeature.Background, baseFeature.ScenarioDefinitions.ToArray(), baseFeature.Comments.ToArray(), sourceFilePath);
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
