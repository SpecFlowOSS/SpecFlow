using System;
using gherkin.parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class StepBuilder : ITableProcessor
    {
        private ScenarioStep step;

        public StepBuilder(string kind, string text, FilePosition position)
        {
            switch (kind.Trim())
            {
                case "Given":
                    step = new Given(new Text(text), null, null);
                    break;
                case "When":
                    step = new When(new Text(text), null, null);
                    break;
                case "Then":
                    step = new Then(new Text(text), null, null);
                    break;
                case "And":
                    step = new And(new Text(text), null, null);
                    break;
                case "But":
                    step = new But(new Text(text), null, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("kind");
            }
            step.FilePosition = position;
        }

        public ScenarioStep GetResult()
        {
            return step;
        }

        public void SetMultilineArg(string text)
        {
            step.MultiLineTextArgument = text;
        }

        public void ProcessTableRow(string[] row, int lineNumber)
        {
            step.AddTableArgRow(row, lineNumber);
        }
    }
}