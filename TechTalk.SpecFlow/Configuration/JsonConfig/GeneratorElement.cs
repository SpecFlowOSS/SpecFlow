using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class GeneratorElement
    {
        [JsonProperty("allowDebugGeneratedFiles", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.AllowDebugGeneratedFiles)]
        public bool AllowDebugGeneratedFiles { get; set; }

        [JsonProperty("allowRowTests", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.AllowRowTests)]
        public bool AllowRowTests { get; set; }


        [JsonProperty("markFeaturesParallelizable", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.MarkFeaturesParallelizable)]
        public bool MarkFeaturesParallelizable { get; set; }


        [JsonProperty("skipParallelizableMarkerForTags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SkipParallelizableMarkerForTags { get; set; }
    }
}