using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Browser;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Configuration
{
    public class RuntimeConfiguration
    {
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
        public bool TraceSuccessfulSteps { get; set; }
        public bool TraceTimings { get; set; }
        public TimeSpan MinTracedDuration { get; set; }
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        public List<string> AdditionalStepAssemblies { get; private set; }

        public RuntimeConfiguration()
        {
            FeatureLanguage = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            ToolLanguage = CultureInfoHelper.GetCultureInfo(ConfigDefaults.FeatureLanguage);
            BindingCulture = null;

            RuntimeUnitTestProvider = ConfigDefaults.UnitTestProviderName;

            DetectAmbiguousMatches = ConfigDefaults.DetectAmbiguousMatches;
            StopAtFirstError = ConfigDefaults.StopAtFirstError;
            MissingOrPendingStepsOutcome = ConfigDefaults.MissingOrPendingStepsOutcome;

            TraceSuccessfulSteps = ConfigDefaults.TraceSuccessfulSteps;
            TraceTimings = ConfigDefaults.TraceTimings;
            MinTracedDuration = TimeSpan.Parse(ConfigDefaults.MinTracedDuration);
            StepDefinitionSkeletonStyle = ConfigDefaults.StepDefinitionSkeletonStyle;

            AdditionalStepAssemblies = new List<string>();
        }

        public void LoadConfiguration()
        {
            UpdateFromQueryString();
        }

        public static IEnumerable<PluginDescriptor> GetPlugins()
        {
            return Enumerable.Empty<PluginDescriptor>(); //TODO: support plugins in Silverlight
        }

        private void UpdateFromQueryString()
        {
            FeatureLanguage = GetCultureInfoFromQueryString("featureLanguage", FeatureLanguage);
            ToolLanguage = GetCultureInfoFromQueryString("toolLanguage", ToolLanguage);

            string providerName;
            if (QueryString.TryGetValue("unitTestProvider", out providerName))
            {
                RuntimeUnitTestProvider = providerName;
            }

            DetectAmbiguousMatches = GetBoolFromQueryString("detectAmbiguousMatches", DetectAmbiguousMatches);
            StopAtFirstError = GetBoolFromQueryString("stopAtFirstError", StopAtFirstError);
            MissingOrPendingStepsOutcome = GetEnumFromQueryString("missingOrPendingStepsOutcome", MissingOrPendingStepsOutcome);

            TraceSuccessfulSteps = GetBoolFromQueryString("traceSuccessfulSteps", TraceSuccessfulSteps);
            TraceTimings = GetBoolFromQueryString("traceTimings", TraceTimings);
            MinTracedDuration = GetTimeSpanFromQueryString("minTracedDuration", MinTracedDuration);
            StepDefinitionSkeletonStyle = GetEnumFromQueryString("stepDefinitionSkeletonStyle", StepDefinitionSkeletonStyle);
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
    }
}
