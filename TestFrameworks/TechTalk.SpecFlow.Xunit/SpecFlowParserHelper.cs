using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.xUnitAdapter.SpecFlowPlugin
{
    public static class SpecFlowParserHelper
    {
        private static SpecFlowGherkinParser CreateParser()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US")); //TODO: use specflow configuration
            return parser;
        }

        public static async Task<SpecFlowDocument> ParseSpecFlowDocumentAsync(string featureFilePath)
        {
            var fileContent = await ReadAllTextAsync(featureFilePath);
            var parser = CreateParser();
            var gherkinDocument = parser.Parse(new StringReader(fileContent), featureFilePath);
            return gherkinDocument;
        }

        public static SpecFlowDocument ParseSpecFlowDocument(string featureFilePath)
        {
            using (var reader = new StreamReader(featureFilePath))
            {
                var parser = CreateParser();
                var gherkinDocument = parser.Parse(reader, featureFilePath);
                return gherkinDocument;
            }
        }

        private static async Task<string> ReadAllTextAsync(string filePath)
        {
            using (var reader = File.OpenText(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static IEnumerable<string> GetTags(this IEnumerable<Tag> tagList)
        {
            if (tagList == null)
                yield break;
            foreach (var tag in tagList)
            {
                yield return tag.Name.TrimStart('@');
            }
        }

        public static string GetExampleRowId(ScenarioOutline scenarioOutline, TableRow exampleRow)
        {
            int exampleRowId = 0;
            foreach (var row in scenarioOutline.Examples.SelectMany(example => example.TableBody))
            {
                ++exampleRowId;
                if (exampleRow == row)
                    return exampleRowId.ToString();
            }
            throw new InvalidOperationException("Unable to find example row");
        }

        public static bool GetExampleRowById(ScenarioOutline scenarioOutline, string exampleRowId, out Examples example, out TableRow exampleRow)
        {
            int id = 0;
            foreach (var ex in scenarioOutline.Examples)
            {
                foreach (var row in ex.TableBody)
                {
                    ++id;
                    if (id.ToString() == exampleRowId)
                    {
                        example = ex;
                        exampleRow = row;
                        return true;
                    }
                }
            }
            example = null;
            exampleRow = null;
            return false;
        }

        public static Dictionary<string, string> GetScenarioOutlineParametersById(ScenarioOutline scenarioOutline, string exampleRowId)
        {
            int id = 0;
            foreach (var example in scenarioOutline.Examples)
            {
                foreach (var row in example.TableBody)
                {
                    ++id;
                    if (id.ToString() == exampleRowId)
                        return GetScenarioOutlineParameters(example, row);
                }
            }
            return null;
        }

        public static Dictionary<string, string> GetScenarioOutlineParameters(Examples example, TableRow exampleRow)
        {
            return example.TableHeader.Cells
                .Zip(exampleRow.Cells, (keyCell, valueCell) => new { Key = keyCell.Value, valueCell.Value })
                .ToDictionary(arg => arg.Key, arg => arg.Value);
        }

        public static Scenario CreateScenario(ScenarioOutline scenarioOutline, Examples example, TableRow exampleRow)
        {
            var parameters = GetScenarioOutlineParameters(example, exampleRow);
            var steps = new List<Step>();

            var tags = new List<Tag>();
            tags.AddRange(scenarioOutline.Tags);
            tags.AddRange(example.Tags);

            foreach (var scenarioOutlineStep in scenarioOutline.Steps.Cast<SpecFlowStep>())
            {
                string stepText = Interpolate(scenarioOutlineStep.Text, parameters);
                var step = new SpecFlowStep(scenarioOutlineStep.Location, scenarioOutlineStep.Keyword, stepText,
                    CreatePickleArguments(scenarioOutlineStep.Argument, parameters),
                    scenarioOutlineStep.StepKeyword, scenarioOutlineStep.ScenarioBlock);
                steps.Add(step);
            }

            return new Scenario(tags.ToArray(), scenarioOutline.Location, scenarioOutline.Keyword, scenarioOutline.Name,
                scenarioOutline.Description, steps.ToArray());
        }

        private static StepArgument CreatePickleArguments(StepArgument argument, Dictionary<string,string> parameters)
        {
            if (argument == null) return null;
            if (argument is DataTable)
            {
                DataTable t = (DataTable)argument;
                var rows = t.Rows;
                var newRows = new List<TableRow>(rows.Count());
                foreach (var row in rows)
                {
                    var cells = row.Cells;
                    var newCells = new List<TableCell>();
                    foreach (var cell in cells)
                    {
                        newCells.Add(new TableCell(cell.Location, Interpolate(cell.Value, parameters)));
                    }
                    newRows.Add(new TableRow(row.Location, newCells.ToArray()));
                }
                return new DataTable(newRows.ToArray());
            }
            if (argument is DocString)
            {
                DocString ds = (DocString)argument;
                return new DocString(
                    ds.Location,
                    ds.ContentType,
                    Interpolate(ds.Content, parameters)
                    );
            }
            throw new InvalidOperationException("Unexpected argument type: " + argument);
        }

        private static string Interpolate(string name, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                name = name.Replace("<" + parameter.Key + ">", parameter.Value);
            }
            return name;
        }

    }
}
