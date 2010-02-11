using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class FeatureBuilder
    {
        private readonly string text;
        private readonly Tags tags;
        private readonly IList<IScenarioBuilder> scenarios = new List<IScenarioBuilder>();
        private BackgroundBuilder background = null;

        public FeatureBuilder(string text, Tags tags)
        {
            this.text = text;
            this.tags = tags;
        }

        public Feature GetResult()
        {
            var textLines = text.Split('\n');
            return new Feature(
                new Text(textLines[0]),
                tags,
                textLines.Skip(1).SkipWhile(line => string.IsNullOrEmpty(line.Trim('\n', '\r', '\t', ' '))).Select(line => new DescriptionLine(new Text(line))).ToArray(),
                background == null ? null : background.GetResult(),
                scenarios.Select(sb => sb.GetResult()).ToArray());
        }

        public void AddScenario(IScenarioBuilder scenarioBuilder)
        {
            scenarios.Add(scenarioBuilder);
        }

        public void AddBackground(BackgroundBuilder backgroundBuilder)
        {
            background = backgroundBuilder;
        }
    }
}