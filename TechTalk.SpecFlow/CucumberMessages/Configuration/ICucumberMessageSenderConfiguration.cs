using System.Collections.Generic;

namespace TechTalk.SpecFlow.CucumberMessages.Configuration
{
    public interface ICucumberMessageSenderConfiguration
    {
        IList<SinkConfigurationEntry> Sinks { get; set; }
    }
}
