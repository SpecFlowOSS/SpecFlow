using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using gherkin;
using java.util;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class GherkinListener : Listener
    {
        public GherkinListener()
        {
            Errors = new List<ErrorDetail>();
        }

        private StepBuilder stepBuilder;
        public List<ErrorDetail> Errors { get; set; }

        public void DisplayRecognitionError(int line, int column, string message)
        {
            string msg = string.Format("({0},{1}): {2}", line, column, message);

            Debug.WriteLine(msg);
            Errors.Add(new ErrorDetail {Message = msg, Row = line, Column = column});
        }

        private FeatureBuilder featureBuilder;
        public Feature GetResult()
        {
            return featureBuilder.GetResult();
        }

        private IList<Tag> tags = new List<Tag>();
        private Tags FlushTags()
        {
            var retval = tags.Any() ? new Tags(tags.ToArray()) : null;
            tags = new List<Tag>();
            return retval;
        }

        private IStepProcessor stepProcessor;
        private ITableProcessor tableProcessor;
        private IExampleProcessor exampleProcessor;

        public void tag(string name, int i)
        {
            tags.Add(new Tag(new Word(name)));
        }

        public void comment(string str, int i)
        {
        }

        public void feature(string keyword, string content, int line_number)
        {
            featureBuilder = new FeatureBuilder(content, FlushTags());
        }

        public void background(string keyword, string content, int line_number)
        {
            var background = new BackgroundBuilder(content, new FilePosition(line_number, 1));
            stepProcessor = background;
            featureBuilder.AddBackground(background);
        }

        public void scenario(string keyword, string content, int line_number)
        {
            var currentScenario = new ScenarioBuilder(content, FlushTags(), new FilePosition(line_number, 1));
            stepProcessor = currentScenario;
            featureBuilder.AddScenario(currentScenario);
        }

        public void scenario_outline(string keyword, string content, int line_number)
        {
            var currentScenario = new ScenarioOutlineBuilder(content, FlushTags(), new FilePosition(line_number, 1));
            stepProcessor = currentScenario;
            exampleProcessor = currentScenario;
            featureBuilder.AddScenario(currentScenario);
        }

        public void examples(string keyword, string content, int line_number)
        {
            var exampleBuilder = new ExampleBuilder(content, new FilePosition(line_number, 1));
            tableProcessor = exampleBuilder;
            exampleProcessor.ProcessExample(exampleBuilder);
        }

        public void step(string keyword, string content, int line_number)
        {
            stepBuilder = new StepBuilder(keyword, content, new FilePosition(line_number, 1));
            tableProcessor = stepBuilder;
            stepProcessor.ProcessStep(stepBuilder);
        }

        public void row(List list, int line_number)
        {
            string[] rows = new string[list.size()];
            list.toArray(rows);
            tableProcessor.ProcessTableRow(rows, line_number);
        }

        public void py_string(string content, int line_number)
        {
            stepBuilder.SetMultilineArg(content);
        }

        public void eof()
        {

        }

        public void syntax_error(string str1, string str2, List l, int line_number)
        {
            string message = "Parse error. Found " + str1 + " when expecting one of: " +
                             "TODO" + ". (Current state: " + str2 + ").";

            DisplayRecognitionError(line_number, 0, message);
        }




//        public void Comment(string content, int i)
//        {
//        }
//
//        public void Feature(Token keyword, Token name)
//        {
//            featureBuilder = new FeatureBuilder(name.Content, FlushTags());
//        }
//
//        public void Background(Token keyword, Token name)
//        {
//            var background = new BackgroundBuilder(name.Content, ToFilePosition(keyword.Position));
//            stepProcessor = background;
//            featureBuilder.AddBackground(background);
//        }
//
//        public void Scenario(Token keyword, Token name)
//        {
//            var currentScenario = new ScenarioBuilder(name.Content, FlushTags(), ToFilePosition(keyword.Position));
//            stepProcessor = currentScenario;
//            featureBuilder.AddScenario(currentScenario);
//        }
//
//        public void ScenarioOutline(Token keyword, Token name)
//        {
//            var currentScenario = new ScenarioOutlineBuilder(name.Content, FlushTags(), ToFilePosition(keyword.Position));
//            stepProcessor = currentScenario;
//            exampleProcessor = currentScenario;
//            featureBuilder.AddScenario(currentScenario);
//        }
//
//        public void Examples(Token keyword, Token name)
//        {
//            var exampleBuilder = new ExampleBuilder(name.Content, ToFilePosition(keyword.Position));
//            tableProcessor = exampleBuilder;
//            exampleProcessor.ProcessExample(exampleBuilder);
//        }
//
//        public void Step(Token keyword, Token name, StepKind stepKind)
//        {
//            stepBuilder = new StepBuilder(stepKind, name.Content, ToFilePosition(keyword.Position));
//            tableProcessor = stepBuilder;
//            stepProcessor.ProcessStep(stepBuilder);
//        }
//
//        public void Table(IList<IList<Token>> rows, Position tablePosition)
//        {
//            tableProcessor.ProcessTable(CreateTable(rows));
//        }
//
//        private Table CreateTable(IList<IList<Token>> rows)
//        {
//            Func<Token, Cell> convertCell = cell => new Cell(new Text(cell.Content)) { FilePosition = ToFilePosition(cell.Position) };
//            Func<IList<Token>, Row> convertRow = row => new Row(row.Select(convertCell).ToArray()) { FilePosition = new FilePosition(row[0].Position.Line, row[0].Position.Column - 1) };
//            return new Table(convertRow(rows[0]), rows.Skip(1).Select(convertRow).ToArray());
//        }
//
//        public void PythonString(Token pyString)
//        {
//            stepBuilder.SetMultilineArg(pyString.Content);
//        }
//
//        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, Position position)
//        {
//            string message = "Parse error. Found " + @event + " when expecting one of: " +
//                             string.Join(", ", legalEvents.ToArray()) + ". (Current state: " + state + ").";
//
//            DisplayRecognitionError(position.Line, position.Column, message);
//        }
//
//        private FilePosition ToFilePosition(Position position)
//        {
//            return new FilePosition(position.Line, position.Column);
//        }
    }

}
