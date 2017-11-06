using System.ComponentModel;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class UnitTestProviderElement
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("NUnit")]
        public string Name { get; set; }
    }
}