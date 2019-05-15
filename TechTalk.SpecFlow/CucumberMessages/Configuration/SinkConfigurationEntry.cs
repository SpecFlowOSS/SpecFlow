using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages.Configuration
{
    public class SinkConfigurationEntry
    {
        public string TypeName { get; set; }

        public IDictionary<string, string> ConfigurationValues { get; set; } = new Dictionary<string, string>();
    }
}
