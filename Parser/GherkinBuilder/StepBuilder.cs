using System;
using gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class StepBuilder : ITableProcessor
    {
        private readonly ScenarioStep step;

        public StepBuilder(string keyword, string text, FilePosition position, I18n i18n)
        {
            if (i18n.keywords("given").contains(keyword)) step = new Given(new Text(text), null, null);
            else if (i18n.keywords("when").contains(keyword)) step = new When(new Text(text), null, null);
            else if (i18n.keywords("then").contains(keyword)) step = new Then(new Text(text), null, null);
            else if (i18n.keywords("and").contains(keyword)) step = new And(new Text(text), null, null);
            else if (i18n.keywords("but").contains(keyword)) step = new But(new Text(text), null, null);
            else throw new ArgumentOutOfRangeException(string.Format("Parameter 'keyword' has value that can not be translated! Value:'{0}'", keyword));

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