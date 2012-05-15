using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration
{
    public class RuntimeConfiguration
    {
        private List<Assembly> _additionalStepAssemblies = new List<Assembly>();

        //language settings
        public CultureInfo FeatureLanguage { get; set; }
        public CultureInfo ToolLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        //unit test framework settings
        public string RuntimeUnitTestProvider { get; set; }

        //runtime settings
        public bool DetectAmbiguousMatches { get; set; }
        public bool StopAtFirstError { get; set; }
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }

        //tracing settings
        public Type TraceListenerType { get; set; }
        public bool TraceSuccessfulSteps { get; set; }
        public bool TraceTimings { get; set; }
        public TimeSpan MinTracedDuration { get; set; }

        public IEnumerable<Assembly> AdditionalStepAssemblies
        {
            get
            {
                return _additionalStepAssemblies;
            }
        }

        public RuntimeConfiguration()
        {
            FeatureLanguage = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            ToolLanguage = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            BindingCulture = null;

            RuntimeUnitTestProvider = ConfigDefaults.UnitTestProviderName;

            DetectAmbiguousMatches = ConfigDefaults.DetectAmbiguousMatches;
            StopAtFirstError = ConfigDefaults.StopAtFirstError;
            MissingOrPendingStepsOutcome = ConfigDefaults.MissingOrPendingStepsOutcome;

            TraceListenerType = typeof(DefaultListener);
            TraceSuccessfulSteps = ConfigDefaults.TraceSuccessfulSteps;
            TraceTimings = ConfigDefaults.TraceTimings;
            MinTracedDuration = TimeSpan.Parse(ConfigDefaults.MinTracedDuration);
        }

        public void LoadConfiguration()
        {
        }

        public static IEnumerable<PluginDescriptor> GetPlugins()
        {
            return Enumerable.Empty<PluginDescriptor>(); //TODO: support plugins
        }
    }
}