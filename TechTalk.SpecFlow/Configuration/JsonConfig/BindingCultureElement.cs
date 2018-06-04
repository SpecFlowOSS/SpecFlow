using System.ComponentModel;
using System.Runtime.Serialization;

//using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class BindingCultureElement
    {
        //[JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("en-US")]
        [DataMember(Name="name")]
        public string Name { get; set; }
    }
}