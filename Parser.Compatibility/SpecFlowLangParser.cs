using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
            var gherkin3Feature = specFlowGherkinParser.Parse(featureFileReader, sourceFilePath);
            return ConvertToCompatibleFeature(gherkin3Feature);
        }

        private Feature ConvertToCompatibleFeature(SpecFlowFeature gherkin3Feature)
        {
            return new Feature(gherkin3Feature.Keyword, gherkin3Feature.Name, 
                ConvertToCompatibleTags(gherkin3Feature.Tags), 
                gherkin3Feature.Description, 
                ConvertToCompatibleBackground(gherkin3Feature.Background), 
                ConvertToCompatibleScenarios(gherkin3Feature.ScenarioDefinitions), 
                ConvertToCompatibleComments(gherkin3Feature.Comments))
            {
                FilePosition = ConvertToCompatibleFilePosition(gherkin3Feature.Location),
                Language = gherkin3Feature.Language,
                SourceFile = gherkin3Feature.SourceFilePath
            };
        }

        private Comment[] ConvertToCompatibleComments(IEnumerable<global::Gherkin.Ast.Comment> comments)
        {
            return comments.Select(ConvertToCompatibleComment).Where(c => c != null).ToArray();
        }

        private Comment ConvertToCompatibleComment(global::Gherkin.Ast.Comment c)
        {
            var trimmedText = c.Text.TrimStart('#', ' ', '\t');
            if (trimmedText.Length == 0)
                return null;
            return new Comment(trimmedText, ConvertToCompatibleFilePosition(c.Location, c.Text.Length - trimmedText.Length));
        }

        private Scenario[] ConvertToCompatibleScenarios(IEnumerable<global::Gherkin.Ast.ScenarioDefinition> scenarioDefinitions)
        {
            return scenarioDefinitions.Select(ConvertToCompatibleScenario).ToArray();
        }

        private Scenario ConvertToCompatibleScenario(global::Gherkin.Ast.ScenarioDefinition sd)
        {
            var result = sd is global::Gherkin.Ast.ScenarioOutline
                ? new ScenarioOutline(sd.Keyword, sd.Name, sd.Description, ConvertToCompatibleTags(sd.Tags), ConvertToCompatibleSteps(sd.Steps), ConvertToCompatibleExamples(((global::Gherkin.Ast.ScenarioOutline)sd).Examples)) //TODO[Gherkin3]: ScenarioOutline compatibility
                : new Scenario(sd.Keyword, sd.Name, sd.Description, ConvertToCompatibleTags(sd.Tags), ConvertToCompatibleSteps(sd.Steps));
            result.FilePosition = ConvertToCompatibleFilePosition(sd.Location);
            return result;
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
            return new GherkinTableRow(tableRow.Cells.Select(c => new GherkinTableCell(c.Value)).ToArray())
            {
                FilePosition = ConvertToCompatibleFilePosition(tableRow.Location)
            };
        }

        private ScenarioSteps ConvertToCompatibleSteps(IEnumerable<global::Gherkin.Ast.Step> steps)
        {
            return new ScenarioSteps(steps.Select(s => ConvertToCompatibleStep((SpecFlowStep)s)).ToArray());
        }

        private ScenarioStep ConvertToCompatibleStep(SpecFlowStep step)
        {
            ScenarioStep result = null;
            if (step.StepKeyword == StepKeyword.Given)
                result = new Given {StepKeyword = step.StepKeyword };
            else if (step.StepKeyword == StepKeyword.When)
                result = new When {StepKeyword = step.StepKeyword };
            else if (step.StepKeyword == StepKeyword.Then)
                result = new Then {StepKeyword = step.StepKeyword };
            else if (step.StepKeyword == StepKeyword.And)
                result = new And { StepKeyword = step.StepKeyword };
            else if (step.StepKeyword == StepKeyword.But)
                result = new But {StepKeyword = step.StepKeyword };

            if (result == null)
                throw new NotSupportedException();

            result.Keyword = step.Keyword;
            result.Text = step.Text;
            result.ScenarioBlock = step.ScenarioBlock;
            result.MultiLineTextArgument = step.Argument is global::Gherkin.Ast.DocString ? ((global::Gherkin.Ast.DocString) step.Argument).Content : null;
            result.TableArg = step.Argument is global::Gherkin.Ast.DataTable ? ConvertToCompatibleTable(((global::Gherkin.Ast.DataTable) step.Argument).Rows) : null;
            result.FilePosition = ConvertToCompatibleFilePosition(step.Location);

            return result;
        }

        private FilePosition ConvertToCompatibleFilePosition(global::Gherkin.Ast.Location location, int columnDiff = 0)
        {
            if (location == null)
                return null;
            return new FilePosition(location.Line, location.Column + columnDiff);
        }

        private GherkinTable ConvertToCompatibleTable(IEnumerable<global::Gherkin.Ast.TableRow> rows)
        {
            var rowsArray = rows.ToArray();
            return new GherkinTable(ConvertToCompatibleRow(rowsArray.First()), rowsArray.Skip(1).Select(ConvertToCompatibleRow).ToArray());
        }

        private Background ConvertToCompatibleBackground(global::Gherkin.Ast.Background background)
        {
            if (background == null)
                return null;

            return new Background(background.Keyword, background.Name, background.Description, ConvertToCompatibleSteps(background.Steps))
            {
                FilePosition = ConvertToCompatibleFilePosition(background.Location)
            };
        }

        private Tags ConvertToCompatibleTags(IEnumerable<global::Gherkin.Ast.Tag> tags)
        {
            if (tags == null || !tags.Any())
                return null;
            return new Tags(tags.Select(t => new Tag(t.GetNameWithoutAt())).ToArray());
        }
    }
}
