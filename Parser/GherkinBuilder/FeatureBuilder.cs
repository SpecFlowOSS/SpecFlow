using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static readonly Regex firstLineRe = new Regex(@"^(?<firstline>[^\r\n]*)[\r\n]+(?<rest>.*)", RegexOptions.Singleline);

        public Feature GetResult()
        {
            string title = text;
            string description = null;

            var match = firstLineRe.Match(text);
            if (match.Success)
            {
                title = match.Groups["firstline"].Value;
                description = match.Groups["rest"].Value;
            }

            return new Feature(
                title,
                tags,
                description,
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