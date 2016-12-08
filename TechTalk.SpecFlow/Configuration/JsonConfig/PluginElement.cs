using Newtonsoft.Json;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class PluginElement
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public string Parameters { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public PluginType Type { get; set; }
    }
}