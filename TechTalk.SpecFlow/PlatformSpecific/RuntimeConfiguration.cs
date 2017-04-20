using System;
using System.Collections.Generic;
#if !DISABLECONFIGFILESUPPORT
using System.Configuration;
#endif
using System.Globalization;
using System.Linq;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Compatibility;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Configuration
{
    public class RuntimeConfiguration
    {
#if !BODI_DISABLECONFIGFILESUPPORT
        public ContainerRegistrationCollection CustomDependencies { get; set; }
#endif

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

        public static IEnumerable<PluginDescriptor> GetPlugins()
        {
#if !DISABLECONFIGFILESUPPORT
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            if (section == null || section.Plugins == null)
                return Enumerable.Empty<PluginDescriptor>();

            return section.Plugins.Select(pce => pce.ToPluginDescriptor());
#else
            return Enumerable.Empty<PluginDescriptor>();
#endif
        }

        public void LoadConfiguration()
        {
#if !DISABLECONFIGFILESUPPORT
            var section = (ConfigurationSectionHandler)ConfigurationManager.GetSection("specFlow");
            if (section != null)
                LoadConfiguration(section);
#endif
        }

#if !DISABLECONFIGFILESUPPORT
        private bool IsSpecified(ConfigurationElement configurationElement)
        {
            return configurationElement != null && configurationElement.ElementInformation.IsPresent;
        }

        public void LoadConfiguration(ConfigurationSectionHandler configSection)
        {
            if (configSection == null) throw new ArgumentNullException("configSection");

            if (IsSpecified(configSection.Language))
            {
                FeatureLanguage = CultureInfoHelper.GetCultureInfo(configSection.Language.Feature);
                this.ToolLanguage = string.IsNullOrEmpty(configSection.Language.Tool) ?
                    CultureInfoHelper.GetCultureInfo(configSection.Language.Feature) :
                    CultureInfoHelper.GetCultureInfo(configSection.Language.Tool);
            }

            if (IsSpecified(configSection.BindingCulture))
            {
                this.BindingCulture = CultureInfoHelper.GetCultureInfo(configSection.BindingCulture.Name);
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
                    this.RuntimeUnitTestProvider = "custom";
                    AddCustomDependency(configSection.UnitTestProvider.RuntimeProvider, typeof(IUnitTestRuntimeProvider), this.RuntimeUnitTestProvider);
                }
                else
                {
                    this.RuntimeUnitTestProvider = configSection.UnitTestProvider.Name;
                }
            }

            if (IsSpecified(configSection.Trace))
            {
                if (!string.IsNullOrEmpty(configSection.Trace.Listener)) // backwards compatibility
                {
                    AddCustomDependency(configSection.Trace.Listener, typeof(ITraceListener));
                }

                this.TraceSuccessfulSteps = configSection.Trace.TraceSuccessfulSteps;
                this.TraceTimings = configSection.Trace.TraceTimings;
                this.MinTracedDuration = configSection.Trace.MinTracedDuration;
                this.StepDefinitionSkeletonStyle = configSection.Trace.StepDefinitionSkeletonStyle;
            }

            foreach (var element in configSection.StepAssemblies)
            {
                var assemblyName = ((StepAssemblyConfigElement)element).Assembly;
                this.AdditionalStepAssemblies.Add(assemblyName);
            }
        }

        private void AddCustomDependency(string implementationType, Type interfaceType, string name = null)
        {
            if (this.CustomDependencies == null)
                this.CustomDependencies = new ContainerRegistrationCollection();

            this.CustomDependencies.Add(implementationType, interfaceType.AssemblyQualifiedName, name);
        }
#endif

#region Equality members

        protected bool Equals(RuntimeConfiguration other)
        {
            return Equals(FeatureLanguage, other.FeatureLanguage) && Equals(ToolLanguage, other.ToolLanguage) && Equals(BindingCulture, other.BindingCulture) && string.Equals(RuntimeUnitTestProvider, other.RuntimeUnitTestProvider) && DetectAmbiguousMatches.Equals(other.DetectAmbiguousMatches) && StopAtFirstError.Equals(other.StopAtFirstError) && MissingOrPendingStepsOutcome.Equals(other.MissingOrPendingStepsOutcome) && TraceSuccessfulSteps.Equals(other.TraceSuccessfulSteps) && TraceTimings.Equals(other.TraceTimings) && MinTracedDuration.Equals(other.MinTracedDuration) && StepDefinitionSkeletonStyle.Equals(other.StepDefinitionSkeletonStyle) &&
                (AdditionalStepAssemblies ?? new List<string>()).SequenceEqual(other.AdditionalStepAssemblies ?? new List<string>());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuntimeConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (FeatureLanguage != null ? FeatureLanguage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ToolLanguage != null ? ToolLanguage.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BindingCulture != null ? BindingCulture.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (RuntimeUnitTestProvider != null ? RuntimeUnitTestProvider.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ DetectAmbiguousMatches.GetHashCode();
                hashCode = (hashCode*397) ^ StopAtFirstError.GetHashCode();
                hashCode = (hashCode*397) ^ MissingOrPendingStepsOutcome.GetHashCode();
                hashCode = (hashCode*397) ^ TraceSuccessfulSteps.GetHashCode();
                hashCode = (hashCode*397) ^ TraceTimings.GetHashCode();
                hashCode = (hashCode*397) ^ MinTracedDuration.GetHashCode();
                hashCode = (hashCode*397) ^ StepDefinitionSkeletonStyle.GetHashCode();
                return hashCode;
            }
        }

#endregion
    }
}