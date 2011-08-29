using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration
{
    public class RuntimeConfiguration
    {
        public ContainerRegistrationCollection CustomDependencies { get; set; }

        private readonly List<Assembly> additionalStepAssemblies = new List<Assembly>();

        //language settings
        public CultureInfo ToolLanguage { get; set; }
        public CultureInfo BindingCulture { get; set; }

        //unit test framework settings
        public string RuntimeUnitTestProvider { get; set; }
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
                return additionalStepAssemblies;
            }
        }

        public RuntimeConfiguration()
        {
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

        public static IEnumerable<PluginDescriptor> GetPlugins()
        {
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            if (section == null || section.Plugins == null)
                return Enumerable.Empty<PluginDescriptor>();

            return section.Plugins.Select(pce => new PluginDescriptor(pce.Name));
        }

        public void LoadConfiguration()
        {
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            LoadConfiguration(section);
        }

        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }

        public void LoadConfiguration(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            if (IsSpecified(configSection.Language))
            {
                this.ToolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ?
                    CultureInfo.GetCultureInfo(configSection.Language.Feature) :
                    CultureInfo.GetCultureInfo(configSection.Language.Tool);
            }

            if (IsSpecified(configSection.BindingCulture))
            {
                this.BindingCulture = CultureInfo.GetCultureInfo(configSection.BindingCulture.Name);
            }

            if (IsSpecified(configSection.Runtime))
            {
                this.DetectAmbiguousMatches = configSection.Runtime.DetectAmbiguousMatches;
                this.StopAtFirstError = configSection.Runtime.StopAtFirstError;
                this.MissingOrPendingStepsOutcome = configSection.Runtime.MissingOrPendingStepsOutcome;

                if (IsSpecified(configSection.Runtime.Dependencies))
                {
                    this.CustomDependencies = configSection.Runtime.Dependencies;
                }
            }

            if (IsSpecified(configSection.UnitTestProvider))
            {
                if (!string.IsNullOrEmpty(configSection.UnitTestProvider.RuntimeProvider))
                {
                    //compatibility mode, we simulate a custom dependency

                    if (this.CustomDependencies == null)
                        this.CustomDependencies = new ContainerRegistrationCollection();

                    this.RuntimeUnitTestProvider = "custom";
                    this.CustomDependencies.Add(configSection.UnitTestProvider.RuntimeProvider, typeof(IUnitTestRuntimeProvider).AssemblyQualifiedName, this.RuntimeUnitTestProvider);
                }
                else
                {
                    this.RuntimeUnitTestProvider = configSection.UnitTestProvider.Name;
                }
            }

            if (IsSpecified(configSection.Trace))
            {
                if (!string.IsNullOrEmpty(configSection.Trace.Listener))
                    this.TraceListenerType = GetTypeConfig(configSection.Trace.Listener);

                this.TraceSuccessfulSteps = configSection.Trace.TraceSuccessfulSteps;
                this.TraceTimings = configSection.Trace.TraceTimings;
                this.MinTracedDuration = configSection.Trace.MinTracedDuration;
            }

            foreach (var element in configSection.StepAssemblies)
            {
                Assembly stepAssembly = Assembly.Load(((StepAssemblyConfigElement)element).Assembly);
                this.additionalStepAssemblies.Add(stepAssembly);
            }
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
    }
}