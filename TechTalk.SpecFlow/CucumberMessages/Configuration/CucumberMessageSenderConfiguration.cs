using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages.Configuration
{
    public class CucumberMessageSenderConfiguration : ICucumberMessageSenderConfiguration
    {
        public IList<SinkConfigurationEntry> Sinks { get; set; } = new List<SinkConfigurationEntry>();
    }
}
