using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

//using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class RuntimeElement
    {
        //[JsonProperty("stopAtFirstError", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "stopAtFirstError")]
        [DefaultValue(ConfigDefaults.StopAtFirstError)]
        public bool StopAtFirstError { get; set; }

        //[JsonProperty("missingOrPendingStepsOutcome", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "missingOrPendingStepsOutcome")]
        [DefaultValue(ConfigDefaults.MissingOrPendingStepsOutcome)]
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }

        //[JsonProperty("obsoleteBehavior", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "obsoleteBehavior")]
        [DefaultValue(ConfigDefaults.ObsoleteBehavior)]
        public ObsoleteBehavior ObsoleteBehavior { get; set; }

        [DataMember(Name = "dependencies")]
        public List<Dependency> Dependencies { get; set; }
    }
}