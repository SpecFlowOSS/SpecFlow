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

        //[JsonProperty("addNonParallelizableMarkerForTags", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "addNonParallelizableMarkerForTags")]
        public List<string> AddNonParallelizableMarkerForTags { get; set; }
    }
}