using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public enum StepKeyword
    {
        Given = 1,
        When = 2,
        Then = 3,
        And = 4,
        But = 5
    }

    public enum ScenarioBlock
    {
        Given = 1,
        When = 2,
        Then = 3,
    }

    public interface IGherkinListener
    {
        void Init(GherkinBuffer buffer, bool isPartialScan);
        void Comment(string commentText, GherkinBufferSpan commentSpan);
        void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void Examples(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void Scenario(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void ScenarioOutline(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void FeatureTag(string name, GherkinBufferSpan tagSpan);
        void ScenarioTag(string name, GherkinBufferSpan tagSpan);
        void ExamplesTag(string name, GherkinBufferSpan tagSpan);
        void Step(string keyword, StepKeyword stepKeyword, ScenarioBlock scenarioBlock, string text, GherkinBufferSpan stepSpan);
        void TableHeader(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans);
        void TableRow(string[] cells, GherkinBufferSpan rowSpan, GherkinBufferSpan[] cellSpans);
        void MultilineText(string text, GherkinBufferSpan textSpan);
        void EOF(GherkinBufferPosition eofPosition);
        void Error(string message, GherkinBufferPosition errorPosition, Exception exception);
    }
}
