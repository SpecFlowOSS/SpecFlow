using System;
using gherkin;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class StepBuilder : ITableProcessor
    {
        private readonly ScenarioStep step;
        private readonly TableBuilder tableBuilder = new TableBuilder();

        public StepBuilder(string keyword, string text, FilePosition position, I18n i18n)
        {
            if (i18n.keywords("and").contains(keyword)) step = new And();
            else if (i18n.keywords("given").contains(keyword)) step = new Given();
            else if (i18n.keywords("when").contains(keyword)) step = new When();
            else if (i18n.keywords("then").contains(keyword)) step = new Then();
            else if (i18n.keywords("but").contains(keyword)) step = new But();
            else throw new ArgumentOutOfRangeException(string.Format("Parameter 'keyword' has value that can not be translated! Value:'{0}'", keyword));

            step.Text = text;
            step.FilePosition = position;
        }

        public StepBuilder(StepKeyword stepKeyword, string text, FilePosition position)
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

            step.Text = text;
            step.FilePosition = position;
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