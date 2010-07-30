using System.Linq;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public enum StepKeyword
    {
        Given,
        When,
        Then,
        And,
        But
    }

    public enum ScenarioBlock
    {
        Given,
        When,
        Then,
    }

    public interface IGherkinListener
    {
        void Comment(string commentText, GherkinBufferSpan commentSpan);
        void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void Examples(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void Scenario(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void ScenarioOutline(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan);
        void FeatureTag(string name, GherkinBufferSpan bufferSpan);
        void ScenarioTag(string name, GherkinBufferSpan bufferSpan);
        void Step(string keyword, StepKeyword stepKeyword, ScenarioBlock scenarioBlock, string text, GherkinBufferSpan stepSpan);
        void TableHeader(string[] cells, GherkinBufferSpan[] cellSpans);
        void TableRow(string[] cells, GherkinBufferSpan[] cellSpans);
        void MultilineText(string text, GherkinBufferSpan textSpan);
        void EOF(GherkinBufferPosition eofPosition);
        void Error(string message, GherkinBufferPosition errorPosition);
    }
}
