using System;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class StepBuilder : ITableProcessor
    {
        private readonly ScenarioStep step;
        private readonly TableBuilder tableBuilder = new TableBuilder();

        public StepBuilder(string keyword, StepKeyword stepKeyword, string text, FilePosition position, ScenarioBlock scenarioBlock)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    step = new Given();
                    break;
                case StepKeyword.When:
                    step = new When();
                    break;
                case StepKeyword.Then:
                    step = new Then();
                    break;
                case StepKeyword.And:
                    step = new And();
                    break;
                case StepKeyword.But:
                    step = new But();
                    break;
                default:
                    throw new NotSupportedException();
            }

            step.Keyword = keyword;
            step.Text = text;
            step.FilePosition = position;
            step.ScenarioBlock = scenarioBlock;
            step.StepKeyword = stepKeyword;
        }

        public ScenarioStep GetResult()
        {
            step.TableArg = tableBuilder.GetResult();

            return step;
        }

        public void SetMultilineArg(string text)
        {
            step.MultiLineTextArgument = text;
        }

        public void ProcessTableRow(string[] cells, FilePosition rowPosition)
        {
            tableBuilder.ProcessTableRow(cells, rowPosition);
        }
    }
}