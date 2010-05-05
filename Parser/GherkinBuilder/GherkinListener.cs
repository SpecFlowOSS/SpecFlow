using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
            _i18n = languageService;
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
        private I18n _i18n;

        public void tag(string name, int i)
        {
            string nameWithoutAt = name.Remove(0, 1);
            tags.Add(new Tag(new Word(nameWithoutAt)));
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

        public void scenarioOutline(string keyword, string content, int line_number)
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
            stepBuilder = new StepBuilder(keyword, content, new FilePosition(line_number, 1), _i18n);
            tableProcessor = stepBuilder;
            stepProcessor.ProcessStep(stepBuilder);
        }

        public void row(List list, int line_number)
        {
            string[] rows = new string[list.size()];
            list.toArray(rows);
            tableProcessor.ProcessTableRow(rows, line_number);
        }

        public void pyString(string content, int line_number)
        {
            stepBuilder.SetMultilineArg(content);
        }

        public void eof()
        {

        }

        public void syntaxError(string str1, string str2, List l, int line_number)
        {
            string message = "Parse error. Found " + str1 + " when expecting one of: " +
                             "TODO" + ". (Current state: " + str2 + ").";

            RegisterError(line_number, null, message);
        }
    }
}
