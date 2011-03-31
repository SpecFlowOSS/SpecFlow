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
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            if (section == null)
                return new RuntimeConfiguration();

            return LoadFromConfigFile(section);
        }

        public static RuntimeConfiguration LoadFromConfigFile(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            var config = new RuntimeConfiguration();
            if (configSection.Language != null)
            {
                config.ToolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ?
                    CultureInfo.GetCultureInfo(configSection.Language.Feature) :
                    CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (configSection.BindingCulture.ElementInformation.IsPresent)
            {
                config.BindingCulture = CultureInfo.GetCultureInfo(configSection.BindingCulture.Name);
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

            foreach (var element in configSection.StepAssemblies)
            {
                Assembly stepAssembly = Assembly.Load(((StepAssemblyConfigElement)element).Assembly);
                config._additionalStepAssemblies.Add(stepAssembly);
            }

            return config;
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
                case "nunit":
                    RuntimeUnitTestProviderType = typeof(NUnitRuntimeProvider);
                    break;
                case "mbunit":
                    RuntimeUnitTestProviderType = typeof(MbUnitRuntimeProvider);
                    break;
                case "xunit":
                    RuntimeUnitTestProviderType = typeof(XUnitRuntimeProvider);
                    break;
                case "mstest":
                    RuntimeUnitTestProviderType = typeof(MsTestRuntimeProvider);
                    break;
                case "mstest.2010":
                    RuntimeUnitTestProviderType = typeof(MsTest2010RuntimeProvider);
                    break;
#if WINDOWS_PHONE
                case "mstest.windowsphone7":
                    RuntimeUnitTestProviderType = typeof(MsTestWP7RuntimeProvider);
                    break;
#endif
                default:
                    RuntimeUnitTestProviderType = null;
                    break;
            }
        }
    }
}