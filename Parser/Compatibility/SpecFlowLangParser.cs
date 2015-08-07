using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Gherkin;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

// ReSharper disable once CheckNamespace
namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowLangParser
    {
        private readonly SpecFlowGherkinParser specFlowGherkinParser;

        public SpecFlowLangParser(CultureInfo defaultLanguage)
        {
            specFlowGherkinParser = new SpecFlowGherkinParser(defaultLanguage);
        }

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            global::Gherkin.Ast.Feature gherkin3Feature;
            try
            {
                gherkin3Feature = specFlowGherkinParser.Parse(featureFileReader, sourceFilePath);
            }
            catch (CompositeParserException compositeParserException)
            {
                throw new SpecFlowParserException(compositeParserException.Errors.Select(ConvertToCompatibleErrorDetail).ToList());
            }
            catch (ParserException parserException)
            {
                throw new SpecFlowParserException(ConvertToCompatibleErrorDetail(parserException));
            }
            var dialect = specFlowGherkinParser.DialectProvider.GetDialect(gherkin3Feature.Language, null);

            Feature feature = ConvertToCompatibleFeature(gherkin3Feature, dialect);

            Debug.Assert(feature != null, "If there were no errors, the feature cannot be null");
            feature.Language = gherkin3Feature.Language;
            feature.SourceFile = sourceFilePath;

            return feature;
        }

        private ErrorDetail ConvertToCompatibleErrorDetail(ParserException parserException)
        {
            return new ErrorDetail
            {
                Message = parserException.Message,
                Line = parserException.Location == null ? null : (int?)parserException.Location.Line,
                Column = parserException.Location == null ? null : (int?)parserException.Location.Column
            };
        }

        private Feature ConvertToCompatibleFeature(global::Gherkin.Ast.Feature gherkin3Feature, global::Gherkin.GherkinDialect dialect)
        {
            return new Feature(gherkin3Feature.Keyword, gherkin3Feature.Name, 
                ConvertToCompatibleTags(gherkin3Feature.Tags), 
                gherkin3Feature.Description, 
                ConvertToCompatibleBackground(gherkin3Feature.Background, dialect), 
                ConvertToCompatibleScenarios(gherkin3Feature.ScenarioDefinitions, dialect), 
                ConvertToCompatibleComments(gherkin3Feature.Comments));
        }

        private Comment[] ConvertToCompatibleComments(IEnumerable<global::Gherkin.Ast.Comment> comments)
        {
            return comments.Select(c => new Comment(c.Text, ConvertToCompatibleFilePosition(c.Location))).ToArray();
        }

        private Scenario[] ConvertToCompatibleScenarios(IEnumerable<global::Gherkin.Ast.ScenarioDefinition> scenarioDefinitions, global::Gherkin.GherkinDialect dialect)
        {
            return scenarioDefinitions.Select(sd => sd is global::Gherkin.Ast.ScenarioOutline
                ? new ScenarioOutline(sd.Keyword, sd.Name, sd.Description, ConvertToCompatibleTags(sd.Tags), ConvertToCompatibleSteps(sd.Steps, dialect), ConvertToCompatibleExamples(((global::Gherkin.Ast.ScenarioOutline)sd).Examples)) //TODO[Gherkin3]: ScenarioOutline compatibility
                : new Scenario(sd.Keyword, sd.Name, sd.Description, ConvertToCompatibleTags(sd.Tags), ConvertToCompatibleSteps(sd.Steps, dialect))).ToArray();
        }

        private Examples ConvertToCompatibleExamples(IEnumerable<global::Gherkin.Ast.Examples> examples)
        {
            return new Examples(examples.Select(e => new ExampleSet(e.Keyword, e.Name, e.Description, ConvertToCompatibleTags(e.Tags), ConvertToCompatibleExamplesTable(e))).ToArray());
        }

        private GherkinTable ConvertToCompatibleExamplesTable(global::Gherkin.Ast.Examples examples)
        {
            return new GherkinTable(ConvertToCompatibleRow(examples.TableHeader), examples.TableBody.Select(ConvertToCompatibleRow).ToArray());
        }

        private GherkinTableRow ConvertToCompatibleRow(global::Gherkin.Ast.TableRow tableRow)
        {
            return new GherkinTableRow(tableRow.Cells.Select(c => new GherkinTableCell(c.Value)).ToArray());
        }

        private ScenarioSteps ConvertToCompatibleSteps(IEnumerable<global::Gherkin.Ast.Step> steps, global::Gherkin.GherkinDialect dialect)
        {
            var block = ScenarioBlock.Given;
            return new ScenarioSteps(steps.Select(s => ConvertToCompatibleStep(s, dialect, ref block)).ToArray());
        }

        private ScenarioStep ConvertToCompatibleStep(global::Gherkin.Ast.Step step, global::Gherkin.GherkinDialect dialect, ref ScenarioBlock block)
        {
            ScenarioStep result = null;
            if (dialect.GivenStepKeywords.Contains(step.Keyword))
            {
                result = new Given {StepKeyword = StepKeyword.Given};
                block = ScenarioBlock.Given;
            }
            else if (dialect.WhenStepKeywords.Contains(step.Keyword))
            {
                result = new When {StepKeyword = StepKeyword.When};
                block = ScenarioBlock.When;
            }
            else if (dialect.ThenStepKeywords.Contains(step.Keyword))
            {
                result = new Then {StepKeyword = StepKeyword.Then};
                block = ScenarioBlock.Then;
            }
            else if (dialect.AndStepKeywords.Contains(step.Keyword))
                result = new And {StepKeyword = StepKeyword.And};
            else if (dialect.ButStepKeywords.Contains(step.Keyword))
                result = new But {StepKeyword = StepKeyword.But};

            if (result == null)
                throw new NotSupportedException();

            result.Keyword = step.Keyword;
            result.Text = step.Text;
            result.ScenarioBlock = block;
            result.MultiLineTextArgument = step.Argument is global::Gherkin.Ast.DocString ? ((global::Gherkin.Ast.DocString) step.Argument).Content : null;
            result.TableArg = step.Argument is global::Gherkin.Ast.DataTable ? ConvertToCompatibleTable(((global::Gherkin.Ast.DataTable) step.Argument).Rows) : null;
            result.FilePosition = ConvertToCompatibleFilePosition(step.Location);

            return result;
        }

        private FilePosition ConvertToCompatibleFilePosition(global::Gherkin.Ast.Location location)
        {
            if (location == null)
                return null;
            return new FilePosition(location.Line, location.Column);
        }

        private GherkinTable ConvertToCompatibleTable(IEnumerable<global::Gherkin.Ast.TableRow> rows)
        {
            var rowsArray = rows.ToArray();
            return new GherkinTable(ConvertToCompatibleRow(rowsArray.First()), rowsArray.Skip(1).Select(ConvertToCompatibleRow).ToArray());
        }

        private Background ConvertToCompatibleBackground(global::Gherkin.Ast.Background background, global::Gherkin.GherkinDialect dialect)
        {
            if (background == null)
                return null;

            return new Background(background.Keyword, background.Name, background.Description, ConvertToCompatibleSteps(background.Steps, dialect));
        }

        private Tags ConvertToCompatibleTags(IEnumerable<global::Gherkin.Ast.Tag> tags)
        {
            if (tags == null)
                return new Tags();
            return new Tags(tags.Select(t => new Tag(t.Name)).ToArray());
        }
    }
}
