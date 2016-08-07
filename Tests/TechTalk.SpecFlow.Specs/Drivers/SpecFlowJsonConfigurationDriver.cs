using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TechTalk.SpecFlow.PlatformSpecific.JsonConfig;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class SpecFlowJsonConfigurationDriver
    {
        private JsonConfig _jsonConfig;

        public SpecFlowJsonConfigurationDriver()
        {
            _jsonConfig = new JsonConfig()
            {
                SpecFlow = new SpecFlowElement()
                {
                    Runtime = new RuntimeElement(),
                    BindingCulture = new BindingCultureElement(),
                    Language = new LanguageElement(),
                    UnitTestProvider = new UnitTestProviderElement(),
                    Plugins = new List<PluginEntry>(),
                    StepAssemblies = new List<StepAssemblyEntry>(),
                    Trace = new TraceElement()
                }
            };
        }

        public bool IsUsed { get; set; }

        public void Save(string filepath)
        {
            var jsonContent = JsonConvert.SerializeObject(_jsonConfig);

            File.WriteAllText(filepath, jsonContent);
        }
    }
}
