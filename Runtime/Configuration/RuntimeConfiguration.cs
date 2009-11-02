using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration
{
    public partial class ConfigurationSectionHandler
    {
        
    }

    internal class RuntimeConfiguration
    {
        static public RuntimeConfiguration Current
        {
            get { return ObjectContainer.Configuration; }
        }

        //language settings
        public CultureInfo ToolLanguage { get; set; }

        //unit test framework settings
        public Type RuntimeUnitTestProviderType { get; set; }

        //runtime settings
        public bool DetectAmbiguousMatches { get; set; }
        public bool StopAtFirstError { get; set; }
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }
        
        //tracing settings
        public Type TraceListenerType { get; set; } //TODO: use it
        public bool TraceSuccessfulSteps { get; set; }
        public bool TraceTimings { get; set; }
        public TimeSpan MinTracedDuration { get; set; }

        public RuntimeConfiguration()
        {
            ToolLanguage = string.IsNullOrEmpty(ConfigDefaults.ToolLanguage) ? 
                CultureInfo.GetCultureInfo(ConfigDefaults.FeatureLanguage) : 
                CultureInfo.GetCultureInfo(ConfigDefaults.ToolLanguage);

            SetUnitTestDefaultsByName(ConfigDefaults.UnitTestProviderName);

            DetectAmbiguousMatches = ConfigDefaults.DetectAmbiguousMatches;
            StopAtFirstError = ConfigDefaults.StopAtFirstError;
            MissingOrPendingStepsOutcome = ConfigDefaults.MissingOrPendingStepsOutcome;

            TraceListenerType = typeof(DefaultTraceListener);
            TraceSuccessfulSteps = ConfigDefaults.TraceSuccessfulSteps;
            TraceTimings = ConfigDefaults.TraceTimings;
            MinTracedDuration = TimeSpan.Parse(ConfigDefaults.MinTracedDuration);
        }

        public static RuntimeConfiguration LoadFromConfigFile()
        {
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            if (section == null)
                return new RuntimeConfiguration();

            return LoadFromConfigFile(section);
        }

        public static RuntimeConfiguration LoadFromConfigFile(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            var config = new RuntimeConfiguration();
            if (configSection.Globalization != null)
            {
                config.ToolLanguage = string.IsNullOrEmpty(configSection.Globalization.ToolLanguage) ?
                    CultureInfo.GetCultureInfo(configSection.Globalization.Language) : 
                    CultureInfo.GetCultureInfo(configSection.Globalization.ToolLanguage);
            }

            if (configSection.UnitTestProvider != null)
            {
                config.SetUnitTestDefaultsByName(configSection.UnitTestProvider.Name);

                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.RuntimeProvider))
                    config.RuntimeUnitTestProviderType = GetTypeConfig(configSection.UnitTestProvider.RuntimeProvider);

                //TODO: config.CheckUnitTestConfig();
            }

            if (configSection.Runtime != null)
            {
                config.DetectAmbiguousMatches = configSection.Runtime.DetectAmbiguousMatches;
                config.StopAtFirstError = configSection.Runtime.StopAtFirstError;
                config.MissingOrPendingStepsOutcome = configSection.Runtime.MissingOrPendingStepsOutcome;
            }

            if (configSection.Trace != null)
            {
                if (!string.IsNullOrEmpty(configSection.Trace.Listener))
                    config.TraceListenerType = GetTypeConfig(configSection.Trace.Listener);

                config.TraceSuccessfulSteps = configSection.Trace.TraceSuccessfulSteps;
                config.TraceTimings = configSection.Trace.TraceTimings;
                config.MinTracedDuration = configSection.Trace.MinTracedDuration;
            }

            return config;
        }

        private static Type GetTypeConfig(string typeName)
        {
            //TODO: nicer error message?
            return Type.GetType(typeName, true);
        }

        private void SetUnitTestDefaultsByName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "nunit":
                    RuntimeUnitTestProviderType = typeof(NUnitRuntimeProvider);
                    break;
                case "mstest":
                    //TODO: RuntimeUnitTestProviderType = typeof(MsTestIntegration);
                    break;
                default:
                    RuntimeUnitTestProviderType = null;
                    break;
            }

        }
    }
}