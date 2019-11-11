using System.Collections.Generic;

namespace TechTalk.SpecFlow.Configuration
{
    public class CucumberMessagesConfiguration
    {
        public bool Enabled { get; set; }
        public List<CucumberMessagesSink> Sinks { get; set; } = new List<CucumberMessagesSink>();
    }

    public class CucumberMessagesSink
    {
        public CucumberMessagesSink(string type, string path)
        {
            Type = type;
            Path = path;
        }

        public string Type { get; }
        public string Path { get;  }
    }
}