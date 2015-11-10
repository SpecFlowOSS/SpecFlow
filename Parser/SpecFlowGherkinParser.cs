using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkin3Parser=Gherkin.Parser;

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

            public override global::Gherkin.GherkinDialect GetDialect(string language, Location location)
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
                        return new global::Gherkin.GherkinDialect(language, languageBaseDialect.FeatureKeywords, languageBaseDialect.BackgroundKeywords, languageBaseDialect.ScenarioKeywords, languageBaseDialect.ScenarioOutlineKeywords, languageBaseDialect.ExamplesKeywords, languageBaseDialect.GivenStepKeywords, languageBaseDialect.WhenStepKeywords, languageBaseDialect.ThenStepKeywords, languageBaseDialect.AndStepKeywords, languageBaseDialect.ButStepKeywords);
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
            this.dialectProvider = new SpecFlowGherkinDialectProvider(defaultLanguage.Name);
        }

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var parser = new Gherkin3Parser(new AstBuilder<Feature>());
            var tokenMatcher = new TokenMatcher(dialectProvider);
            var feature = parser.Parse(new TokenScanner(featureFileReader), tokenMatcher);
            //TODO[Gherkin3]: add source file path

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
