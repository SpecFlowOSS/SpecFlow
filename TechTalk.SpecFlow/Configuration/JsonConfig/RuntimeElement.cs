using System.ComponentModel;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class RuntimeElement
    {
        [JsonProperty("stopAtFirstError", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.StopAtFirstError)]
        public bool StopAtFirstError { get; set; }


        [JsonProperty("missingOrPendingStepsOutcome", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.MissingOrPendingStepsOutcome)]
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }
    }
}