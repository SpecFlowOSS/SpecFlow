using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration
{
    public partial class ConfigurationSectionHandler
    {

    }

    internal class RuntimeConfiguration
    {
        private List<Assembly> _additionalStepAssemblies = new List<Assembly>();

        static public RuntimeConfiguration Current
        {
            get { return ObjectContainer.Configuration; }
        }

        //language settings
        public CultureInfo ToolLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        //unit test framework settings
        public Type RuntimeUnitTestProviderType { get; set; }

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
            ToolLanguage = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            BindingCulture = null;

            SetUnitTestDefaultsByName(ConfigDefaults.UnitTestProviderName);

            DetectAmbiguousMatches = ConfigDefaults.DetectAmbiguousMatches;
            StopAtFirstError = ConfigDefaults.StopAtFirstError;
            MissingOrPendingStepsOutcome = ConfigDefaults.MissingOrPendingStepsOutcome;

            TraceListenerType = typeof(DefaultListener);
            TraceSuccessfulSteps = ConfigDefaults.TraceSuccessfulSteps;
            TraceTimings = ConfigDefaults.TraceTimings;
            MinTracedDuration = TimeSpan.Parse(ConfigDefaults.MinTracedDuration);
        }

        public static RuntimeConfiguration GetConfig()
        {
            return new RuntimeConfiguration();
        }

        private static Type GetTypeConfig(string typeName)
        {
            try
            {
                return Type.GetType(typeName, true);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Invalid type reference '{0}': {1}",
                        typeName, ex.Message), ex);
            }
        }

        private void SetUnitTestDefaultsByName(string name)
        {
            switch (name.ToLower())
            {
                case "mstest.windowsphone7":
                    RuntimeUnitTestProviderType = typeof(MsTestWP7RuntimeProvider);
                    break;
                default:
                    RuntimeUnitTestProviderType = null;
                    break;
            }
        }
    }
}