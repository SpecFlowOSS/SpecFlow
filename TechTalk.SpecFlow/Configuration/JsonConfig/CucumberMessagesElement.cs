using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class CucumberMessagesElement
    {
        [DefaultValue(false)]
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "sinks")]
        public List<CucumberMessageSinkElement> Sinks { get; set; }
    }
}