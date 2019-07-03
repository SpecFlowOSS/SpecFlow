using System;

namespace TechTalk.SpecFlow
{
    public class ScenarioInfo
    {
        public string[] Tags { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public ScenarioInfo(string title, string description, params string[] tags)
        {
            Title = title;
            Description = description;
            Tags = tags ?? new string[0];
        }
    }
}