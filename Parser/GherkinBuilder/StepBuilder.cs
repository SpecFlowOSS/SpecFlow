using System;
using Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class StepBuilder : ITableProcessor
    {
        private ScenarioStep step;

        public StepBuilder(StepKind kind, string text, FilePosition position)
        {
            switch (kind)
            {
                case StepKind.Given:
                    step = new Given(new Text(text), null, null);
                    break;
                case StepKind.When:
                    step = new When(new Text(text), null, null);
                    break;
                case StepKind.Then:
                    step = new Then(new Text(text), null, null);
                    break;
                case StepKind.And:
                    step = new And(new Text(text), null, null);
                    break;
                case StepKind.But:
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

        public void ProcessTable(Table table)
        {
            step.TableArg = table;
        }
    }
}