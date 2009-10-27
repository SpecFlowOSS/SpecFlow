using System;
using System.Linq;

namespace TechTalk.SpecFlow
{
    public class FeatureInfo
    {
        public string[] Tags { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public FeatureInfo(string title, string description, params string[] tags)
        {
            Title = title;
            Description = description;
            Tags = tags;
        }
    }
}