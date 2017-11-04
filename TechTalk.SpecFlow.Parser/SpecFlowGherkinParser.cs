using System;
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
        private readonly IGherkinDialectProvider dialectProvider;

        private class SpecFlowGherkinDialectProvider : GherkinDialectProvider
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
                        return new GherkinDialect(language, languageBaseDialect.FeatureKeywords, languageBaseDialect.BackgroundKeywords, languageBaseDialect.ScenarioKeywords, languageBaseDialect.ScenarioOutlineKeywords, languageBaseDialect.ExamplesKeywords, languageBaseDialect.GivenStepKeywords, languageBaseDialect.WhenStepKeywords, languageBaseDialect.ThenStepKeywords, languageBaseDialect.AndStepKeywords, languageBaseDialect.ButStepKeywords);
                    }
                }

                return base.GetDialect(language, location);
            }
        }

        public IGherkinDialectProvider DialectProvider
        {
            get { return dialectProvider; }
        }

        public SpecFlowGherkinParser(IGherkinDialectProvider dialectProvider)
        {
            this.dialectProvider = dialectProvider;
        }

        public SpecFlowGherkinParser(CultureInfo defaultLanguage) 
            : this(new SpecFlowGherkinDialectProvider(defaultLanguage.Name))
        {
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

        private class SpecFlowAstBuilder : AstBuilder<SpecFlowDocument>
        {
            private readonly string sourceFilePath;
            private ScenarioBlock scenarioBlock = ScenarioBlock.Given;

            public SpecFlowAstBuilder(string sourceFilePath)
            {
                this.sourceFilePath = sourceFilePath;
            }

            protected override Feature CreateFeature(Tag[] tags, Location location, string language, string keyword, string name, string description, ScenarioDefinition[] children, AstNode node)
            {
                return new SpecFlowFeature(tags, location, language, keyword, name, description, children);
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

            protected override GherkinDocument CreateGherkinDocument(Feature feature, Comment[] gherkinDocumentComments, AstNode node)
            {
                return new SpecFlowDocument((SpecFlowFeature)feature, gherkinDocumentComments, sourceFilePath);
            }

            protected override Scenario CreateScenario(Tag[] tags, Location location, string keyword, string name, string description, Step[] steps, AstNode node)
            {
                ResetBlock();
                return base.CreateScenario(tags, location, keyword, name, description, steps, node);
            }

            protected override ScenarioOutline CreateScenarioOutline(Tag[] tags, Location location, string keyword, string name, string description, Step[] steps, Examples[] examples, AstNode node)
            {
                ResetBlock();
                return base.CreateScenarioOutline(tags, location, keyword, name, description, steps, examples, node);
            }

            protected override Background CreateBackground(Location location, string keyword, string name, string description, Step[] steps, AstNode node)
            {
                ResetBlock();
                return base.CreateBackground(location, keyword, name, description, steps, node);
            }
        }

        public SpecFlowDocument Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var parser = new Parser<SpecFlowDocument>(CreateAstBuilder(sourceFilePath));
            SpecFlowDocument specFlowDocument = parser.Parse(CreateTokenScanner(featureFileReader), CreateTokenMatcher());

            CheckSemanticErrors(specFlowDocument);

            return specFlowDocument;
        }

        protected virtual ITokenScanner CreateTokenScanner(TextReader featureFileReader)
        {
            return new TokenScanner(featureFileReader);
        }

        protected virtual ITokenMatcher CreateTokenMatcher()
        {
            return new TokenMatcher(dialectProvider);
        }

        protected virtual IAstBuilder<SpecFlowDocument> CreateAstBuilder(string sourceFilePath)
        {
            return new SpecFlowAstBuilder(sourceFilePath);
        }

        protected virtual void CheckSemanticErrors(SpecFlowDocument specFlowDocument)
        {
            if (specFlowDocument?.SpecFlowFeature == null)
                return;

            var errors = new List<ParserException>();

            CheckForDuplicateScenarios(specFlowDocument.SpecFlowFeature, errors);

            CheckForDuplicateExamples(specFlowDocument.SpecFlowFeature, errors);

            CheckForMissingExamples(specFlowDocument.SpecFlowFeature, errors);

            // collect
            if (errors.Count == 1)
                throw errors[0];
            if (errors.Count > 1)
                throw new CompositeParserException(errors.ToArray());
        }

        private void CheckForDuplicateScenarios(SpecFlowFeature feature, List<ParserException> errors)
        {
            // duplicate scenario name
            var duplicatedScenarios = feature.ScenarioDefinitions.GroupBy(sd => sd.Name, sd => sd).Where(g => g.Count() > 1).ToArray();
            errors.AddRange(
                duplicatedScenarios.Select(g =>
                    new SemanticParserException(
                        string.Format("Feature file already contains a scenario with name '{0}'", g.Key),
                        g.ElementAt(1).Location)));
        }

        private void CheckForDuplicateExamples(SpecFlowFeature feature, List<ParserException> errors)
        {
            foreach (var scenarioDefinition in feature.ScenarioDefinitions)
            {
                var scenarioOutline = scenarioDefinition as ScenarioOutline;
                if (scenarioOutline != null)
                {
                    var duplicateExamples = scenarioOutline.Examples
                                                           .Where(e => !String.IsNullOrWhiteSpace(e.Name))
                                                           .Where(e => e.Tags.All(t => t.Name != "ignore"))
                                                           .GroupBy(e => e.Name, e => e).Where(g => g.Count() > 1);

                    foreach (var duplicateExample in duplicateExamples)
                    {
                        var message = string.Format("Scenario Outline '{0}' already contains an example with name '{1}'", scenarioOutline.Name, duplicateExample.Key);
                        var semanticParserException = new SemanticParserException(message, duplicateExample.ElementAt(1).Location);
                        errors.Add(semanticParserException);
                    }
                }
            }
        }

        private void CheckForMissingExamples(SpecFlowFeature feature, List<ParserException> errors)
        {
            foreach (var scenarioDefinition in feature.ScenarioDefinitions)
            {
                var scenarioOutline = scenarioDefinition as ScenarioOutline;
                if (scenarioOutline != null)
                {
                    if (DoesntHavePopulatedExamples(scenarioOutline))
                    {
                        var message = string.Format("Scenario Outline '{0}' has no examples defined", scenarioOutline.Name);
                        var semanticParserException = new SemanticParserException(message, scenarioDefinition.Location);
                        errors.Add(semanticParserException);
                    }
                }
            }
        }

        private static bool DoesntHavePopulatedExamples(ScenarioOutline scenarioOutline)
        {
            return !scenarioOutline.Examples.Any() || scenarioOutline.Examples.Any(x => x.TableBody == null || !x.TableBody.Any());
        }
    }
}
