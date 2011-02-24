using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class FeatureBuilder
    {
        private string keyword;
        private string title;
        private string description;
        private string sourceFilePath;
        private Tags tags;
        private readonly IList<IScenarioBuilder> scenarios = new List<IScenarioBuilder>();
        private BackgroundBuilder background = null;
        protected FilePosition position;
        private readonly List<Comment> comments = new List<Comment>();

        public string SourceFilePath
        {
            get { return sourceFilePath; }
            set { sourceFilePath = value; }
        }

        public void SetHeader(string keyword, string title, string description, Tags tags, FilePosition position)
        {
            this.keyword = keyword;
            this.title = title;
            this.description = description;
            this.tags = tags;
            this.position = position;
        }

        public Feature GetResult()
        {
            var scenarioResults = scenarios.Select(sb => sb.GetResult()).ToArray();

            comments.Sort((c1, c2) => c1.FilePosition.Line.CompareTo(c2.FilePosition.Line));

            var feature = new Feature(
                keyword,
                title,
                tags,
                description,
                background == null ? null : background.GetResult(),
                scenarioResults,
                comments.ToArray());

            feature.SourceFile = sourceFilePath;
            feature.FilePosition = position;
            return feature;
        }

        public void AddScenario(IScenarioBuilder scenarioBuilder)
        {
            if (scenarios.Any(s => s.Title.Equals(scenarioBuilder.Title)))
                throw new GherkinSemanticErrorException(
                    string.Format("Feature file already contains a scenario with name '{0}'", scenarioBuilder.Title),
                    scenarioBuilder.Position);

            scenarios.Add(scenarioBuilder);
        }

        public void AddBackground(BackgroundBuilder backgroundBuilder)
        {
            if (background != null)
                throw new GherkinSemanticErrorException(
                    "Feature file already contains a background section.",
                    backgroundBuilder.Position);
            background = backgroundBuilder;
        }

        public void AddComment(string commentText, FilePosition filePosition)
        {
            comments.Add(new Comment(commentText, filePosition));
        }
    }
}