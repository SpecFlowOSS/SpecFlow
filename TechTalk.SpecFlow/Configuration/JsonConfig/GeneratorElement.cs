using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class GeneratorElement
    {
        //[JsonProperty("allowDebugGeneratedFiles", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "allowDebugGeneratedFiles")]
        [DefaultValue(ConfigDefaults.AllowDebugGeneratedFiles)]
        public bool AllowDebugGeneratedFiles { get; set; }

        //[JsonProperty("allowRowTests", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.AllowRowTests)]
        [DataMember(Name = "allowRowTests")]
        public bool AllowRowTests { get; set; }


        //[JsonProperty("markFeaturesParallelizable", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.MarkFeaturesParallelizable)]
        [DataMember(Name = "markFeaturesParallelizable")]
        public bool MarkFeaturesParallelizable { get; set; }


        //[JsonProperty("skipParallelizableMarkerForTags", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "skipParallelizableMarkerForTags")]
        public List<string> SkipParallelizableMarkerForTags { get; set; }
    }
}