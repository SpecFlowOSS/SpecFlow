using System;
using System.Linq;

namespace TechTalk.SpecFlow
{
    public class ScenarioInfo
    {
        public string[] Tags { get; private set; }
        public string Title { get; private set; }

        public ScenarioInfo(string title, params string[] tags)
        {
            Title = title;
            Tags = tags ?? new string[0];
        }
    }
}