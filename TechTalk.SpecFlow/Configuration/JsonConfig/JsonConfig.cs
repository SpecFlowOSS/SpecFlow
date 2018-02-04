using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class JsonConfig
    {
        [JsonProperty(PropertyName = "specflow", NullValueHandling = NullValueHandling.Ignore)]
        public SpecFlowElement SpecFlow { get; set; }
    }
}