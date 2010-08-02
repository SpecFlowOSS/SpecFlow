using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class GherkinParserListener : IGherkinListener
    {
        private readonly string filePath;

        private readonly FeatureBuilder featureBuilder = new FeatureBuilder();
        private StepBuilder stepBuilder;

        private IStepProcessor stepProcessor;
        private ITableProcessor tableProcessor;
        private IExampleProcessor exampleProcessor;

        private IList<Tag> tags = new List<Tag>();

        private readonly List<ErrorDetail> errors = new List<ErrorDetail>();

        public GherkinParserListener(string filePath)
        {
            this.filePath = filePath;
        }

        public List<ErrorDetail> Errors
        {
            get { return errors; }
        }

        public void RegisterError(int? line, int? column, string message)
        {
            var errorDetail = new ErrorDetail { Message = message, Line = line, Column = column };
            RegisterError(errorDetail);
        }

        public void RegisterError(ErrorDetail errorDetail)
        {
            //Debug.WriteLine(string.Format("({0},{1}): {2}", errorDetail.Line, errorDetail.Column, errorDetail.Message));
            Errors.Add(errorDetail);
        }

        public Feature GetResult()
        {
            return featureBuilder.GetResult();
        }

        private Tags FlushTags()
        {
            var retval = tags.Any() ? new Tags(tags.ToArray()) : null;
            tags = new List<Tag>();
            return retval;
        }

        private FilePosition GetFilePosition(GherkinBufferPosition startPosition)
        {
            //return new FilePosition(startPosition.Line + 1, startPosition.LinePosition + 1);
            return new FilePosition(startPosition.Line + 1);
        }

        public void Comment(string commentText, GherkinBufferSpan commentSpan)
        {
            //nop;
        }

        public void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            featureBuilder.SetHeader(name, description, FlushTags());
            featureBuilder.SourceFilePath = filePath;
        }

        public void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            var background = new BackgroundBuilder(name, description, GetFilePosition(headerSpan.StartPosition));
            stepProcessor = background;
            featureBuilder.AddBackground(background);
        }

        public void Examples(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            var exampleBuilder = new ExampleBuilder(name, description, GetFilePosition(headerSpan.StartPosition));
            tableProcessor = exampleBuilder;
            exampleProcessor.ProcessExample(exampleBuilder);
        }

        public void Scenario(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            var currentScenario = new ScenarioBuilder(name, description, FlushTags(), GetFilePosition(headerSpan.StartPosition));
            stepProcessor = currentScenario;
            featureBuilder.AddScenario(currentScenario);
        }

        public void ScenarioOutline(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            var currentScenario = new ScenarioOutlineBuilder(name, description, FlushTags(), GetFilePosition(headerSpan.StartPosition));
            stepProcessor = currentScenario;
            exampleProcessor = currentScenario;
            featureBuilder.AddScenario(currentScenario);
        }

        private void ProcessTag(string name)
        {
            string nameWithoutAt = name.Substring(1);
            tags.Add(new Tag(nameWithoutAt));
        }

        public void FeatureTag(string name, GherkinBufferSpan bufferSpan)
        {
            ProcessTag(name);
        }

        public void ScenarioTag(string name, GherkinBufferSpan bufferSpan)
        {
            ProcessTag(name);
        }

        public void Step(string keyword, StepKeyword stepKeyword, ScenarioBlock scenarioBlock, string text, GherkinBufferSpan stepSpan)
        {
            stepBuilder = new StepBuilder(stepKeyword, text, GetFilePosition(stepSpan.StartPosition));
            tableProcessor = stepBuilder;
            stepProcessor.ProcessStep(stepBuilder);
        }

        public void TableHeader(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans)
        {
            tableProcessor.ProcessTableRow(cells, GetFilePosition(rowSpan.StartPosition));
        }

        public void TableRow(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans)
        {
            tableProcessor.ProcessTableRow(cells, GetFilePosition(rowSpan.StartPosition));
        }

        public void MultilineText(string text, GherkinBufferSpan textSpan)
        {
            stepBuilder.SetMultilineArg(text);
        }

        public void EOF(GherkinBufferPosition eofPosition)
        {
            //nop
        }

        public void Error(string message, GherkinBufferPosition errorPosition, Exception exception)
        {
            RegisterError(errorPosition.Line + 1, errorPosition.LinePosition + 1, message);
        }
    }
}