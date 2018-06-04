using System.ComponentModel;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class LanguageElement
    {
        //[JsonProperty("feature", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("en")]
        [DataMember(Name="feature")]
        public string Feature { get; set; }
    }
}