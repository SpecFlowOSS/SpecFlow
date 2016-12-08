using System.ComponentModel;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class LanguageElement
    {
        [JsonProperty("feature", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("en")]
        public string Feature { get; set; }
    }
}