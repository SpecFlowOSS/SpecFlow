using System;
using System.Collections.Specialized;

namespace TechTalk.SpecFlow
{
    public class ScenarioInfo
    {
        public string[] Tags { get; private set; }
        public IOrderedDictionary Arguments { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public ScenarioInfo(string title, string description, string[] tags, IOrderedDictionary arguments)
        {
            Title = title;
            Description = description;
            Tags = tags ?? new string[0];
            Arguments = arguments;
        }
    }
}