using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class StepAssemblyElement
    {
        [JsonProperty("assembly", NullValueHandling = NullValueHandling.Ignore)]
        public string Assembly { get; set; }
    }
}