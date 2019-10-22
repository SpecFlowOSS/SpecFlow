using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class CucumberMessageSinkElement
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }
    }
}