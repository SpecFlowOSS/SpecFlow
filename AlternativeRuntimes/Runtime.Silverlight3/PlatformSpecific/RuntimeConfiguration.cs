using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Browser;

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
            var configuration = new RuntimeConfiguration();
            configuration.UpdateFromQueryString();
            return configuration;
        }

        private void UpdateFromQueryString()
        {
            ToolLanguage = GetCultureInfoFromQueryString("toolLanguage", ToolLanguage);

            string providerName;
            if (QueryString.TryGetValue("unitTestProvider", out providerName))
                SetUnitTestDefaultsByName(providerName);

            DetectAmbiguousMatches = GetBoolFromQueryString("detectAmbiguousMatches", DetectAmbiguousMatches);
            StopAtFirstError = GetBoolFromQueryString("stopAtFirstError", StopAtFirstError);
            MissingOrPendingStepsOutcome = GetEnumFromQueryString("missingOrPendingStepsOutcome", MissingOrPendingStepsOutcome);

            TraceListenerType = GetTypeFromQueryString("traceListener", TraceListenerType);
            TraceSuccessfulSteps = GetBoolFromQueryString("traceSuccessfulSteps", TraceSuccessfulSteps);
            TraceTimings = GetBoolFromQueryString("traceTimings", TraceTimings);
            MinTracedDuration = GetTimeSpanFromQueryString("minTracedDuration", MinTracedDuration);
        }

        private static CultureInfo GetCultureInfoFromQueryString(string key, CultureInfo defaultValue)
        {
            string value;
            return QueryString.TryGetValue(key, out value) ? CultureInfoHelper.GetCultureInfo(value) : defaultValue;
        }

        private static bool GetBoolFromQueryString(string key, bool defaultValue)
        {
            string value;
            bool boolValue;
            if (QueryString.TryGetValue(key, out value) && bool.TryParse(value, out boolValue))
            {
                return boolValue;
            }

            return defaultValue;
        }

        private static T GetEnumFromQueryString<T>(string key, T defaultValue)
        {
            string value;
            if (QueryString.TryGetValue(key, out value))
            {
                try
                {
                    var enumValue = (T)Enum.Parse(typeof(T), value, false);
                    return enumValue;
                }
                catch
                {
                }
            }

            return defaultValue;
        }

        private static Type GetTypeFromQueryString(string key, Type defaultValue)
        {
            string value;
            return QueryString.TryGetValue(key, out value) ? Type.GetType(value, true) : defaultValue;
        }

        private static TimeSpan GetTimeSpanFromQueryString(string key, TimeSpan defaultValue)
        {
            string value;
            TimeSpan timeSpanValue;
            if (QueryString.TryGetValue(key, out value) && TimeSpan.TryParse(value, out timeSpanValue))
            {
                return timeSpanValue;
            }

            return defaultValue;
        }

        protected static IDictionary<string, string> QueryString
        {
            get { return HtmlPage.Document.QueryString; }
        }

        private void SetUnitTestDefaultsByName(string name)
        {
            switch (name.ToLower())
            {
                case "mstest.silverlight":
                case "mstest.silverlight3":
                case "mstest.silverlight4":
                    RuntimeUnitTestProviderType = typeof (MsTestSilverlightRuntimeProvider);
                    break;

                default:
                    RuntimeUnitTestProviderType = null;
                    break;
            }
        }
    }
}
