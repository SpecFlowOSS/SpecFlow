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
        public GherkinListener(I18n languageService)
        {
            i18n = languageService;
            Errors = new List<ErrorDetail>();
        }

        private StepBuilder stepBuilder;
        public List<ErrorDetail> Errors { get; set; }

        public void RegisterError(int? line, int? column, string message)
        {
            var errorDetail = new ErrorDetail { Message = message, Line = line, Column = column };
            RegisterError(errorDetail);
        }

        public void RegisterError(ErrorDetail errorDetail)
        {
            Debug.WriteLine(string.Format("({0},{1}): {2}", errorDetail.Line, errorDetail.Column, errorDetail.Message));
            Errors.Add(errorDetail);
        }

        private readonly FeatureBuilder featureBuilder = new FeatureBuilder();

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
        private readonly I18n i18n;

        public void tag(string name, int i)
        {
            string nameWithoutAt = name.Remove(0, 1);
            tags.Add(new Tag(nameWithoutAt));
        }

        public void comment(string comment, int line)
        {
        }

        public void location(string uri, int offset)
        {
            //TODO
            featureBuilder.SourceFilePath = uri;
        }

        public void feature(string keyword, string name, string description, int line)
        {
            featureBuilder.SetHeader(name, description, FlushTags());
        }

        public void background(string keyword, string name, string description, int line)
        {
            var background = new BackgroundBuilder(name, description, new FilePosition(line));
            stepProcessor = background;
            featureBuilder.AddBackground(background);
        }

        public void scenario(string keyword, string name, string description, int line)
        {
            var currentScenario = new ScenarioBuilder(name, description, FlushTags(), new FilePosition(line));
            stepProcessor = currentScenario;
            featureBuilder.AddScenario(currentScenario);
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            var currentScenario = new ScenarioOutlineBuilder(name, description, FlushTags(), new FilePosition(line));
            stepProcessor = currentScenario;
            exampleProcessor = currentScenario;
            featureBuilder.AddScenario(currentScenario);
        }

        public void examples(string keyword, string name, string description, int line)
        {
            var exampleBuilder = new ExampleBuilder(name, description, new FilePosition(line));
            tableProcessor = exampleBuilder;
            exampleProcessor.ProcessExample(exampleBuilder);
        }

        public void step(string keyword, string text, int line)
        {
            stepBuilder = new StepBuilder(keyword, text, new FilePosition(line), i18n);
            tableProcessor = stepBuilder;
            stepProcessor.ProcessStep(stepBuilder);
        }

        public void row(List list, int line)
        {
            string[] rows = new string[list.size()];
            list.toArray(rows);
            tableProcessor.ProcessTableRow(rows, new FilePosition(line));
        }

        public void pyString(string content, int line)
        {
            stepBuilder.SetMultilineArg(content);
        }

        public void eof()
        {

        }

        public void syntaxError(string state, string eventName, List legalEvents, int line)
        {
            string message = string.Format("Parser error on line {2}. State: {0}, Event: {1}", state, eventName, line);

            RegisterError(line, null, message);
        }
    }
}
